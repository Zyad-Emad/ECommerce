using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Application.Common
{
    public class Result
    {
        public bool IsSuccess { get; }
        public IReadOnlyList<Error> Errors { get; }
        protected Result(bool isSuccess , IReadOnlyList<Error> errors)
        {
            IsSuccess = isSuccess;
            Errors = errors;
        }
        public static Result Ok() => new(true , Array.Empty<Error>());
        public static Result Fail(Error error) => new(false ,new[] {error });
        public static Result Fail(IReadOnlyList<Error> errors) => new(false ,errors);

    }
    public class Result<TValue> : Result
    {
        private readonly TValue value;

        public TValue data => IsSuccess ? value : throw new InvalidOperationException("Can Not Access Value Of Failed Result");
        private Result(TValue value) : base(true , Array.Empty<Error>())
        {
            this.value = value;
        }
        private Result(Error error) : base(false, new[] {error})
        {
            value = default!;
        }
        private Result(IReadOnlyList<Error> errors) : base(false, errors)
        {
            value = default!;
        }
        public static Result<TValue> Ok(TValue value) => new Result<TValue>(value);
        public static Result<TValue> Fail(Error error) => new Result<TValue>(error);
        public static Result<TValue> Fail(IReadOnlyList<Error> errors) => new Result<TValue>(errors);
    }
}
