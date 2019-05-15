using Autofac;
using NUnit.Framework;
using System;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Wikiled.Delfi.AcceptanceTests.Helper;
using Wikiled.Delfi.Readers;
using Wikiled.News.Monitoring.Data;
using Wikiled.News.Monitoring.Readers;

namespace Wikiled.Delfi.AcceptanceTests.Readers
{
    [TestFixture]
    public class CommentsReaderReaderTests
    {
        private NetworkHelper helper;

        [SetUp]
        public void SetUp()
        {
            helper = new NetworkHelper();
        }

        [TearDown]
        public void Teardown()
        {
            helper.Dispose();
        }

        [TestCase]
        public async Task ReadComments()
        {
            var article = new ArticleDefinition();
            article.Id = "80585701";
            article.Url = new Uri("https://www.delfi.lt/auto/patarimai/siulo-keliuose-statyti-naujo-tipo-matuoklius-gales-daugiau-nei-isivaizduojate.d?id=80585701");
            var instance = helper.Container.Resolve<ICommentsReader>(new TypedParameter(typeof(ArticleDefinition), article));
            var comments = await instance.ReadAllComments().ToLookup(item => item.Id);
            Assert.Greater(comments.Count, 100);
            Assert.AreEqual(comments.Count, ((CommentsReader)instance).Total);
        }
    }
}
