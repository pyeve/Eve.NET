using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;

[assembly:InternalsVisibleTo("Eve.Tests")]
[assembly:InternalsVisibleTo("Test")]

namespace Eve
{
    // TODO: Goal is to rename EC to EveClient.
    public static class EC
    { 

        public static GetMany<T> Get<T>()
        {
            return new GetMany<T>();
        }
        public class GetMany<T>
        {
            private string _where;
            private string _ifModifiedSince;
            private string _deleted;
            public GetMany<T> Where(Expression<Func<T, bool>> predicate)
            {
                _where = WhereTranslator.Translate(predicate.Body);
                return this;
            }
            public GetMany<T> IfModifiedSince(DateTime date)
            {
                var prop = TypeDescriptor
                    .GetProperties(typeof(T), new Attribute[] { new RemoteAttribute(Meta.LastUpdated)})[0];

                var propertyName = 
                    (prop.Attributes.OfType<JsonPropertyAttribute>().Any())
                        ? ((JsonPropertyAttribute)prop.Attributes[typeof(JsonPropertyAttribute)]).PropertyName
                        : prop.Name;

                _ifModifiedSince = $"\"{propertyName}\": \"{date.ToString("r")}\"";

                return this;
            }
            public GetMany<T> IncludeDeleted()
            {
                _deleted = "show_deleted";
                return this;
            }
            internal string BuildQuery()
            {
                var wherePart = (_ifModifiedSince != null) ? string.Join(", ", _where, _ifModifiedSince) : _where;
                var where =  $"{{{wherePart}}}";

                var query = (_deleted != null) ? string.Join("?", where, _deleted) : where;

                return query;
            }
            public List<T> Execute()
            {
                // TODO: perform the request and return actual results.
                var query = BuildQuery();
                return new List<T>();
            }
        }
    }
}
