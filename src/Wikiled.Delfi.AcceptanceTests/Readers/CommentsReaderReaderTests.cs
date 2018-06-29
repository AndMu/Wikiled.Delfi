using System;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging.Abstractions;
using NUnit.Framework;
using Wikiled.Delfi.Adjusters;
using Wikiled.Delfi.Data;
using Wikiled.Delfi.Readers;

namespace Wikiled.Delfi.AcceptanceTests.Readers
{
    [TestFixture]
    public class CommentsReaderReaderTests
    {
        [TestCase]
        public async Task ReadComments()
        {
            ArticleDefinition article = new ArticleDefinition();
            article.Url = new Uri("https://www.delfi.lt/veidai/zmones/petro-grazulio-dukreles-mama-paviesino-ju-susirasinejima-galite-suprasti-kaip-jauciausi.d?id=78390475");
            var reader = new CommentsReader(new NullLoggerFactory(), article, new AnonymousAdjuster(), new HtmlReader());
            await reader.Init();
            var comments = await reader.ReadAllComments().ToArray();
            Assert.Greater(comments.Length, 100);
        }
    }
}
