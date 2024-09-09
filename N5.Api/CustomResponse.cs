namespace N5.Api
{
    public class CustomResponse<T>
    {
        public int Status { get; set; }
        public T? Data { get; set; }
        public List<string>? Errors { get; set; }

        public CustomResponse() { }

        public CustomResponse(int status, T data)
        {
            Status = status;
            Data = data;
        }

        public CustomResponse(int status, string mensaje)
        {
            Status = status;
            Errors = mensaje.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries).ToList();
        }
    }
}
