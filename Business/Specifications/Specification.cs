using System.Linq.Expressions;

namespace Business.Specifications
{
    /// <summary>
    /// Specification pattern for building reusable, composable query filters.
    /// Encapsulates WHERE, INCLUDE, ORDER BY, PAGING logic in a single object.
    /// </summary>
    /// <typeparam name="T">The entity type being queried.</typeparam>
    public class Specification<T> where T : class
    {
        /// <summary>
        /// The WHERE clause predicate.
        /// </summary>
        public Expression<Func<T, bool>>? Criteria { get; set; }

        /// <summary>
        /// List of navigation properties to eager-load.
        /// </summary>
        public List<Expression<Func<T, object>>> Includes { get; } = new();

        /// <summary>
        /// List of string-based navigation properties for complex includes.
        /// </summary>
        public List<string> IncludeStrings { get; } = new();

        /// <summary>
        /// The ORDER BY clause expression.
        /// </summary>
        public Expression<Func<T, object>>? OrderBy { get; set; }

        /// <summary>
        /// The ORDER BY DESCENDING clause expression.
        /// </summary>
        public Expression<Func<T, object>>? OrderByDescending { get; set; }

        /// <summary>
        /// Page number for paging (1-based).
        /// </summary>
        public int? PageNumber { get; set; }

        /// <summary>
        /// Page size for paging.
        /// </summary>
        public int? PageSize { get; set; }

        /// <summary>
        /// Whether to track entities (false for read-only queries).
        /// </summary>
        public bool IsTrackingDisabled { get; set; } = false;

        /// <summary>
        /// Adds a WHERE criteria to the specification.
        /// </summary>
        public virtual void AddCriteria(Expression<Func<T, bool>> criteria)
        {
            Criteria = criteria;
        }

        /// <summary>
        /// Adds an eager-load navigation property.
        /// </summary>
        public virtual void AddInclude(Expression<Func<T, object>> includeExpression)
        {
            Includes.Add(includeExpression);
        }

        /// <summary>
        /// Adds a string-based eager-load navigation property.
        /// </summary>
        public virtual void AddInclude(string includeString)
        {
            IncludeStrings.Add(includeString);
        }

        /// <summary>
        /// Adds an ORDER BY clause.
        /// </summary>
        public virtual void AddOrderBy(Expression<Func<T, object>> orderByExpression)
        {
            OrderBy = orderByExpression;
        }

        /// <summary>
        /// Adds an ORDER BY DESCENDING clause.
        /// </summary>
        public virtual void AddOrderByDescending(Expression<Func<T, object>> orderByDescendingExpression)
        {
            OrderByDescending = orderByDescendingExpression;
        }

        /// <summary>
        /// Adds paging to the specification.
        /// </summary>
        public virtual void ApplyPaging(int pageNumber, int pageSize)
        {
            PageNumber = pageNumber;
            PageSize = pageSize;
        }

        /// <summary>
        /// Disables entity tracking for this specification (for read-only queries).
        /// </summary>
        public virtual void DisableTracking()
        {
            IsTrackingDisabled = true;
        }
    }
}
