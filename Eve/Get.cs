using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace Eve
{
    public static class EC
    {
        public static Getter<T> Get<T>()
        {
            return new Getter<T>();
        }
        public class Getter<T>
        {
            public Getter() { }
            public List<T> Where(Expression<Func<T, bool>> predicate)
            {
                var param = predicate.Parameters[0];
                var operation = (BinaryExpression)predicate.Body;
                var left = operation.Left;
                var right = operation.Right;
                return new List<T>();

            }
        }
    }
}
