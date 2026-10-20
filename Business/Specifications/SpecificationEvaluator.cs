using Microsoft.EntityFrameworkCore;

namespace Business.Specifications
{
    /// <summary>
    /// Applies a Specification<T> to an IQueryable<T> to build complex queries.
    /// Handles WHERE, INCLUDE, ORDER BY, and PAGING.
    /// </summary>
    public static class SpecificationEvaluator<T> where T : class
    {
        /// <summary>
        /// Applies the specification to a queryable.
        /// </summary>
        public static IQueryable<T> GetQuery(IQueryable<T> inputQuery, Specification<T> specification)
        {
            var query = inputQuery;

            // Apply WHERE criteria
            if (specification.Criteria is not null)
            {
                query = query.Where(specification.Criteria);
            }

            // Apply string-based includes first
            query = specification.IncludeStrings.Aggregate(query, (current, include) =>
                current.Include(include));

            // Apply expression-based includes
            query = specification.Includes.Aggregate(query, (current, include) =>
                current.Include(include));

            // Apply ORDER BY
            if (specification.OrderBy is not null)
            {
                query = query.OrderBy(specification.OrderBy);
            }

            // Apply ORDER BY DESCENDING
            if (specification.OrderByDescending is not null)
            {
                query = query.OrderByDescending(specification.OrderByDescending);
            }

            // Apply PAGING
            if (specification.PageNumber is not null && specification.PageSize is not null)
            {
                var skip = (specification.PageNumber.Value - 1) * specification.PageSize.Value;
                query = query.Skip(skip).Take(specification.PageSize.Value);
            }

            // Apply tracking settings
            if (specification.IsTrackingDisabled)
            {
                query = query.AsNoTracking();
            }

            return query;
        }
    }
}
