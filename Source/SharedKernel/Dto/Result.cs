namespace DrakeLambert.Peerra.WebApi.SharedKernel.Dto
{
    public class Result
    {
        public bool Succeeded { get; protected set; }

        public bool Failed => !Succeeded;

        public string Error { get; protected set; }

        public Result InnerResult { get; protected set; }

        public bool HasInnerResult => InnerResult != null;

        protected Result(bool succeeded = true, string error = null, Result innerResult = null)
        {
            Succeeded = succeeded;
            Error = error;
            InnerResult = innerResult;
        }

        public static Result Success()
        {
            return new Result();
        }

        public static Result Fail(string error, Result innerResult = null)
        {
            return new Result(false, error);
        }
    }

    public class Result<TValue> : Result
    {
        public TValue Value { get; set; }

        private Result(bool succeeded = true, string error = null, Result innerResult = null, TValue value = default(TValue)) : base(succeeded, error, innerResult)
        {
            Value = value;
        }

        public static Result<TValue> Success(TValue value)
        {
            return new Result<TValue>(true, value: value);
        }

        public static new Result<TValue> Fail(string error, Result innerResult = null)
        {
            return new Result<TValue>(false, error, innerResult);
        }
    }
}
