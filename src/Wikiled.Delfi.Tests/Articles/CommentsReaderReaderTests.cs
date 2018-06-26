using System;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging.Abstractions;
using NUnit.Framework;
using Wikiled.Delfi.Articles;

namespace Wikiled.Delfi.Tests.Articles
{
    [TestFixture]
    public class CommentsReaderReaderTests
    {
        [TestCase]
        public async Task ReadComments()
        {
            ArticleDefinition article = new ArticleDefinition();
            article.Url = new Uri("https://www.delfi.lt/veidai/zmones/petro-grazulio-dukreles-mama-paviesino-ju-susirasinejima-galite-suprasti-kaip-jauciausi.d?id=78390475");
            var reader = new CommentsReader(new NullLoggerFactory(), article, new AnonymousAdjuster());
            await reader.Init();
            var comments = await reader.ReadAllComments().ToArray();
            var distinct = comments.Select(item => item.Id).Distinct();
        }
    }
}
