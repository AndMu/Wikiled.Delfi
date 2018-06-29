using System;
using Moq;
using NUnit.Framework;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Reactive.Testing;
using Wikiled.Common.Utilities.Config;
using Wikiled.Delfi.Data;
using Wikiled.Delfi.Feeds;
using Wikiled.Delfi.Monitoring;
using Wikiled.Delfi.Readers;

namespace Wikiled.Delfi.Tests.Monitoring
{
    [TestFixture]
    public class ArticlesMonitorTests : ReactiveTest
    {
        private TestScheduler scheduler;

        private Mock<IFeedsHandler> mockFeedsHandler;

        private Mock<IApplicationConfiguration> configuration;

        private Mock<IArticleDataReader> articleDataReader;

        private ArticlesMonitor instance;

        private Article articleResult;

        private ArticleDefinition definition;

        private ArticleDefinition[] definitions;

        [SetUp]
        public void SetUp()
        {
            scheduler = new TestScheduler();
            configuration = new Mock<IApplicationConfiguration>();
            configuration.Setup(item => item.Now).Returns(DateTime.UtcNow);
            definition = new ArticleDefinition();
            definition.Id = "2";
            definitions = new[] {definition};
            articleResult = new Article(definition, new CommentsData(1, new CommentData[] { }), new CommentsData(1, new CommentData[] { }), new ArticleText(), DateTime.UtcNow);
            mockFeedsHandler = new Mock<IFeedsHandler>();
            articleDataReader = new Mock<IArticleDataReader>();
            instance = CreateArticlesMonitor();
        }

        [Test]
        public void StartSimple()
        {
            SetupReader();
            var observer = scheduler.CreateObserver<Article>();
            instance.Start().Subscribe(observer);
            scheduler.AdvanceBy(1);
            scheduler.AdvanceBy(TimeSpan.FromHours(2).Ticks);
            observer.Messages.AssertEqual(OnNext<Article>(0, article => article.Definition == definition));
        }

        [Test]
        public void StartDifferent()
        {
            SetupReader();
            var observer = scheduler.CreateObserver<Article>();
            instance.Start().Subscribe(observer);
            scheduler.AdvanceBy(1);
            definition.Id = "3";
            scheduler.AdvanceBy(TimeSpan.FromHours(6).Ticks);
            observer.Messages.AssertEqual(
                OnNext<Article>(0, article => article.Definition == definition),
                OnNext<Article>(TimeSpan.FromHours(1).Ticks, article => article.Definition == definition));
        }

        [TestCase(true)]
        [TestCase(false)]
        public void StartRefreshNoChange(bool hasChanged)
        {
            SetupReader();
            articleDataReader.Setup(item => item.RequiresRefreshing(articleResult)).Returns(Task.FromResult(hasChanged));
            var observer = scheduler.CreateObserver<Article>();
            instance.Start().Subscribe(observer);
            scheduler.AdvanceBy(TimeSpan.FromHours(6).Ticks);
            if (hasChanged)
            {
                observer.Messages.AssertEqual(
                    OnNext<Article>(0, article => article.Definition == definition),
                    OnNext<Article>(TimeSpan.FromHours(6).Ticks, article => article.Definition == definition));
            }
            else
            {
                observer.Messages.AssertEqual(OnNext<Article>(0, article => article.Definition == definition));
            }
        }

        [Test]
        public void Construct()
        {
            Assert.Throws<ArgumentNullException>(() => new ArticlesMonitor(null, new NullLoggerFactory(), scheduler, mockFeedsHandler.Object, articleDataReader.Object));
            Assert.Throws<ArgumentNullException>(() => new ArticlesMonitor(configuration.Object, null, scheduler, mockFeedsHandler.Object, articleDataReader.Object));
            Assert.Throws<ArgumentNullException>(() => new ArticlesMonitor(configuration.Object, new NullLoggerFactory(), null, mockFeedsHandler.Object, articleDataReader.Object));
            Assert.Throws<ArgumentNullException>(() => new ArticlesMonitor(configuration.Object, new NullLoggerFactory(), scheduler, null, articleDataReader.Object));
            Assert.Throws<ArgumentNullException>(() => new ArticlesMonitor(configuration.Object, null, scheduler, mockFeedsHandler.Object, articleDataReader.Object));
        }

        private void SetupReader()
        {
            articleDataReader.Setup(item => item.Read(definition)).Returns(Task.FromResult(articleResult));
            mockFeedsHandler.Setup(item => item.GetArticles()).Returns(definitions.ToObservable());
        }

        private ArticlesMonitor CreateArticlesMonitor()
        {
            return new ArticlesMonitor(configuration.Object, new NullLoggerFactory(), scheduler, mockFeedsHandler.Object, articleDataReader.Object);
        }
    }
}