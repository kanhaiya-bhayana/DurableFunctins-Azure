namespace Test.Dto
{
    public class PartsResponse
    {
        public PartsResponse()
        {
            MyParts = new List<string>();
        }
        public List<string> MyParts { get; set; }
    }
}