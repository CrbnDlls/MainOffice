using MainOffice.Models;
using Microsoft.CSharp;
using System;
using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Validation;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace MainOffice.Functions
{
    public class BootsrapTableServerDataFormat
    {
        public BootsrapTableServerDataFormat(object SorceList, int Total, int TotalNotFiltered)
        {
            rows = SorceList;
            total = Total;
            totalNotFiltered = TotalNotFiltered;
        }

        public int total { get; set; }
        public int totalNotFiltered { get; set; }
        public object rows { get; set; }
    }

    public static class Function
    {
        public static IQueryable<T> OrderBy<T>(this IQueryable<T> source, string ordering, params object[] values)
        {
            var type = typeof(T);
            PropertyInfo property = null;
            ParameterExpression parameter = null;
            MemberExpression propertyAccess = null;
            if (ordering.Contains("."))
            {
                property = type.GetProperty(ordering.Substring(0,ordering.IndexOf(".")));
                parameter = Expression.Parameter(type, "p");
                propertyAccess = Expression.MakeMemberAccess(parameter, property);
                
                property = property.PropertyType.GetProperty(ordering.Substring(ordering.IndexOf(".") + 1));
                propertyAccess = Expression.MakeMemberAccess(propertyAccess, property);
            }
            else
            {
                property = type.GetProperty(ordering);
                parameter = Expression.Parameter(type, "p");
                propertyAccess = Expression.MakeMemberAccess(parameter, property);
            }

            

            var orderByExp = Expression.Lambda(propertyAccess, parameter);
            MethodCallExpression resultExp;
            if (values[0].ToString() == "desc")
            {
                resultExp = Expression.Call(typeof(Queryable), "OrderByDescending", new Type[] { type, property.PropertyType }, source.Expression, Expression.Quote(orderByExp));
            }
            else
            {
                resultExp = Expression.Call(typeof(Queryable), "OrderBy", new Type[] { type, property.PropertyType }, source.Expression, Expression.Quote(orderByExp));
            }
            return source.Provider.CreateQuery<T>(resultExp);
        }

        public static IQueryable<T> WhereFilter<T>(this IQueryable<T> source, string parameterToFilter, params int[] values)
        {
            ParameterExpression pe = Expression.Parameter(typeof(T), "e");
            Expression left = Expression.Property(pe, parameterToFilter);
            Expression conditions = null;
            
            for (int i = 0; i < values.Length; i++)
            {
                Expression right = Expression.Constant(values[i]);
                if (values[i] == 0)
                    right = Expression.Constant(null);
                if (left.Type == typeof(int?))
                    right = Expression.Convert(right, typeof(int?));
                Expression e1 = Expression.Equal(left, right);
                if (i == 0)
                {
                    conditions = e1;
                }
                else
                {
                    conditions = Expression.Or(conditions, e1);
                }
            }
            return source.Where(Expression.Lambda<Func<T, bool>>(conditions, new[] { pe }));
        }

        //public static string ToLocalDateFormat(string Date)
        //{
        //    DateTime sampleDate = new DateTime(1976, 08, 02);
        //    string sample = sampleDate.ToShortDateString();

        //    return null;
        //}
        public static string EncryptString(string plainText)
        {
            string key = "b14ca5898a4e4133bbce2ea2315a1916";
            byte[] iv = new byte[16];
            byte[] array;

            using (Aes aes = Aes.Create())
            {
                aes.Key = Encoding.UTF8.GetBytes(key);
                aes.IV = iv;

                ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

                using (MemoryStream memoryStream = new MemoryStream())
                {
                    using (CryptoStream cryptoStream = new CryptoStream((Stream)memoryStream, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter streamWriter = new StreamWriter((Stream)cryptoStream))
                        {
                            streamWriter.Write(plainText);
                        }

                        array = memoryStream.ToArray();
                    }
                }
            }

            return Convert.ToBase64String(array);
        }

        public static string DecryptString(string cipherText)
        {
            string key = "b14ca5898a4e4133bbce2ea2315a1916";
            byte[] iv = new byte[16];
            byte[] buffer = Convert.FromBase64String(cipherText);

            using (Aes aes = Aes.Create())
            {
                aes.Key = Encoding.UTF8.GetBytes(key);
                aes.IV = iv;
                ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

                using (MemoryStream memoryStream = new MemoryStream(buffer))
                {
                    using (CryptoStream cryptoStream = new CryptoStream((Stream)memoryStream, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader streamReader = new StreamReader((Stream)cryptoStream))
                        {
                            return streamReader.ReadToEnd();
                        }
                    }
                }
            }
        }

        public static async Task<string[]> SaveChangesToDb(DbContext db)
        {
            string[] result = new string[2];
            result[0] = "error";

            try
            {
                await db.SaveChangesAsync();
                result[0] = "success";
                //if (x== 0)
                //{
                //    result[1] = "Нет изменений";
                //}
                //else
                //{
                //    result[0] = "success";
                //}
            }
            catch (DbEntityValidationException e)
            {
                result[1] = e.InnerException != null ? e.InnerException.InnerException != null ? e.InnerException.InnerException.Message : e.InnerException.Message : e.Message;
            }
            catch (DbUpdateConcurrencyException e)
            {
                result[0] = "concurrencyError";
                var entry = e.Entries.Single();
                var databaseEntry = entry.GetDatabaseValues();
                if (databaseEntry == null)
                {
                    result[1] = "Не возможно сохранить изменения. Запись была удалена другим пользователем, после того, как вы получили начальные данные.";
                }
                else
                {
                    result[1] = "Запись, которую вы попытались изменить, "
                    + "была изменена другим пользователем после того, как вы получили начальные данные. "
                    + "Ваша операция была отменена.";//e.InnerException != null ? e.InnerException.InnerException != null ? e.InnerException.InnerException.Message : e.InnerException.Message : e.Message;
                }
                await entry.ReloadAsync();
            }
            catch (DbUpdateException e)
            {
                result[1] = e.InnerException != null ? e.InnerException.InnerException != null ? e.InnerException.InnerException.Message : e.InnerException.Message : e.Message;
            }
            return result;
        }
        public static bool EqualId(this object objA, object objB)
        {
            if (objA == null) return false;
            if (objB == null) return false;

            var obj1Type = objA.GetType();
            
            var valueA = obj1Type.GetProperty("Id").GetValue(objA, null).ToString();
            var valueB = obj1Type.GetProperty("Id").GetValue(objB, null).ToString();
            
            if (valueA == valueB)
            {
                return true;
            }

            return false;
        }
    }

    public interface IObjectExtender
    {
        object Extend(object obj1, object obj2);
    }

    public class ObjectExtender : IObjectExtender
    {
        private readonly IDictionary<Tuple<Type, Type>, Assembly>
            _cache = new Dictionary<Tuple<Type, Type>, Assembly>();

        public object Extend(object obj1, object obj2)
        {
            if (obj1 == null) return obj2;
            if (obj2 == null) return obj1;

            var obj1Type = obj1.GetType();
            var obj2Type = obj2.GetType();

            var values = obj1Type.GetProperties()
                .ToDictionary(
                    property => property.Name,
                    property => property.GetValue(obj1, null));

            string additionalClass = values.ContainsKey("class") ? values["class"].ToString() : null;

            foreach (var property in obj2Type.GetProperties())
            {
                if (property.Name == "class" & additionalClass != null)
                {
                    values[property.Name] = property.GetValue(obj2, null).ToString() + " " + additionalClass;
                }
                else if (values.ContainsKey(property.Name))
                {
                    continue;
                }
                else
                { 
                    values.Add(property.Name, property.GetValue(obj2, null));
                }
            }
            // check for cached
            var key = Tuple.Create(obj1Type, obj2Type);
            if (!_cache.ContainsKey(key))
            {
                // create assembly containing merged type
                var codeProvider = new CSharpCodeProvider();
                var code = new StringBuilder();

                code.Append("public class mergedType{ \n");
                foreach (var propertyName in values.Keys)
                {
                    // use object for property type, avoids assembly references
                    code.Append(
                        string.Format(
                            "public object @{0}{{ get; set;}}\n",
                            propertyName));
                }
                code.Append("}");

                var compilerResults = codeProvider.CompileAssemblyFromSource(
                    new CompilerParameters
                    {
                        CompilerOptions = "/optimize /t:library",
                        GenerateInMemory = true
                    },
                    code.ToString());

                _cache.Add(key, compilerResults.CompiledAssembly);
            }

            var merged = _cache[key].CreateInstance("mergedType");
            Debug.Assert(merged != null, "merged != null");

            // copy data
            foreach (var propertyInfo in merged.GetType().GetProperties())
            {
                propertyInfo.SetValue(
                    merged,
                    values[propertyInfo.Name],
                    null);
            }

            return merged;
        }
    }

    public static class Extensions
    {
        public static IEnumerable<T> EmptyIfNull<T>(this IEnumerable<T> source)
        {
            return source ?? Enumerable.Empty<T>();
        }
    }
}