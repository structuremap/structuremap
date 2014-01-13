using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.Serialization;
using System.Security.Cryptography;

namespace StructureMap.Building
{

    [Serializable]
    public class StructureMapBuildException : StructureMapException
    {

        public static Expression Wrap(Type returnType, Expression expression, string description)
        {
            var genericParameter = Expression.Parameter(typeof (Exception), "ex");
            var newSmEx = Expression.New(Constructor, Expression.Constant(description), genericParameter);

            var genericThrow = Expression.Throw(newSmEx);

            var genericCatch = Expression.Catch(genericParameter, Expression.Block(genericThrow, Expression.Default(returnType)));

            var smParameter = Expression.Parameter(typeof (StructureMapBuildException), "ex");

            var smPush = Expression.Call(smParameter, PushMethod, Expression.Constant(description));
            var rethrow = Expression.Throw(smParameter);

            var pushAndReThrowBlock = Expression.Block(returnType, smPush, rethrow, Expression.Default(returnType));
            var smCatch = Expression.Catch(smParameter, pushAndReThrowBlock);


            return Expression.TryCatch(expression, smCatch, genericCatch);
        }


        public static void ThrowExpression()
        {
            var good = Expression.Constant("I am good");

            var throwGeneral = Expression.Block(Expression.Throw(Expression.Constant(new NotImplementedException())),
                Expression.Constant("bar"));

            var smEx = new StructureMapBuildException("you stink!");
            var throwSpecific = Expression.Block(Expression.Throw(Expression.Constant(smEx)),
                Expression.Constant("bar"));


            var wrapped = Wrap(typeof (string), throwSpecific, "Trying out good");
            try
            {
                var text = Expression.Lambda<Func<string>>(wrapped).Compile()();
                Debug.WriteLine(text);
            }
            catch (StructureMapBuildException e)
            {
                Console.WriteLine(e);
            }

        }

        public static void Try()
        {


            Expression<Func<string>> expr = () => default(string);
            Debug.WriteLine(expr);


            var block = Expression.Block(typeof(string), Expression.Throw(Expression.Constant(new DivideByZeroException())), Expression.Constant("foo"));

            var unaryExpression = Expression.Throw(Expression.Constant(new NotImplementedException()), typeof(string));
            var tryCatch = Expression.TryCatch(block,
                Expression.Catch(typeof (Exception), unaryExpression));

            // Expression.Constant("I blew up")

                var text = Expression.Lambda<Func<string>>(tryCatch).Compile()();
                Debug.WriteLine(text);



           
        }


        private readonly Queue<string> _descriptions = new Queue<string>();

        public StructureMapBuildException(string message, Exception innerException) : base(null, innerException)
        {
            _descriptions.Enqueue(message);
        }

        public StructureMapBuildException(string message) : base(message)
        {
            _descriptions.Enqueue(message);
        }

        protected StructureMapBuildException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }


    }
}