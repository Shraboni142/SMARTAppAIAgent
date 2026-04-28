using PagedList;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;

namespace SMART.Web.Repositories
{
    public interface IRepository<T> where T : class
    {
        void Add(T entity);
        void Update(T entity);
        void Delete(T entity);
        void Delete(Expression<Func<T, bool>> where);
        T GetById(long id);
        T GetById(int id);
        T GetById(string id);
        T Get(Expression<Func<T, bool>> where);
        T Get(Expression<Func<T, bool>> where, bool isUseAsNoTracking);
        IEnumerable<T> GetAll();
        List<T> GetAll_List();
        IEnumerable<T> GetMany(Expression<Func<T, bool>> where);
        IEnumerable<T> GetMany_With_AsNoTracking(Expression<Func<T, bool>> where);
        List<T> GetMany_List(Expression<Func<T, bool>> where);
        IEnumerable<T> GetMany<TOrder>(Expression<Func<T, bool>> where, int pageNumber, int pageSize, Expression<Func<T, TOrder>> orderBy, out int totalRecords);
        IPagedList<T> GetPage<TOrder>(Page page, Expression<Func<T, bool>> where, Expression<Func<T, TOrder>> order);
        IPagedList<T> GetPagedDescending<TOrder>(Page page, Expression<Func<T, bool>> where, Expression<Func<T, TOrder>> orderByDescending);
        IEnumerable<T> ExecWithStoreProcedure(string query, params object[] parameters);
        IEnumerable<U> GetBy<U>(Expression<Func<T, bool>> exp, Expression<Func<T, U>> columns);
        IEnumerable<TU> GetBy<TU, TOrder>(Expression<Func<T, bool>> Where_exp, Expression<Func<T, TU>> columns, int pageNumber, int pageSize, Expression<Func<T, TOrder>> orderBy, out int totalRecords);
        int GetCount(Expression<Func<T, bool>> where);
        void AddMultiple(IEnumerable<T> list);
        string Add_Bulk_Insert(IEnumerable<T> list);
        void DeleteMultiple(IEnumerable<T> list);
        int ExecuteCommand(string sqlCommand, params object[] parameters);
        IEnumerable<T> ExecStoreProcedure<T>(string sql, params object[] parameters);
        IEnumerable<T> SQLQueryList<T>(string sql);
        IEnumerable<T> SQLQueryList<T>(string sql, params object[] parameters);
        T SQLQuery<T>(string sql);
        T SQLQuery<T>(string sql, params SqlParameter[] parameters);
        bool IsExist(Expression<Func<T, bool>> predicate);
        T ExecuteScalar<T>(string sqlCommand, params object[] parameters);

        bool Any(Expression<Func<T, bool>> predicate);
        T First(Expression<Func<T, bool>> predicate);
        T FirstOrDefault(Expression<Func<T, bool>> predicate);
        string Max(Expression<Func<T, string>> predicate);
        string Min(Expression<Func<T, string>> predicate);

    }

    public class Page
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }

        public Page()
        {
            PageNumber = 1;
            PageSize = 10;
        }

        public Page(int pageNumber, int pageSize)
        {
            PageNumber = pageNumber;
            PageSize = pageSize;
        }

        public int Skip
        {
            get { return (PageNumber - 1) * PageSize; }
        }
    }

    public static class PagingExtensions
    {
        /// <summary>
        /// Extend IQueryable TO simplify access TO skip and take methods 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="queryable"></param>
        /// <param name="page"></param>
        /// <returns>IQueryable with Skip and Take having been performed</returns>
        public static IQueryable<T> GetPage<T>(this IQueryable<T> queryable, Page page)
        {
            return queryable.Skip(page.Skip).Take(page.PageSize);
        }
    }
}
