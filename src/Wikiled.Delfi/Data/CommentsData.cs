namespace Wikiled.Delfi.Data
{
    public class CommentsData
    {
        public CommentsData(int expected, CommentData[] comments)
        {
            Expected = expected;
            Comments = comments ?? throw new System.ArgumentNullException(nameof(comments));
        }

        public int Expected { get; }

        public CommentData[] Comments { get; }
    }
}
