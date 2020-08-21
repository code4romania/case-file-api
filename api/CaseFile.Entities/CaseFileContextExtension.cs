﻿using LinqKit;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace CaseFile.Entities
{
    public partial class CaseFileContext : DbContext
    {
        public CaseFileContext(DbContextOptions<CaseFileContext> options)
            : base(options)
        {

        }
    }

    public static class EfBuilderExtensions
    {
        /// <summary>
        /// super simple and dumb translation of .Contains because is not supported pe EF plus
        /// this translates to contains in EF SQL
        /// </summary>
        /// <param name="source"></param>
        /// <param name="contains"></param>
        /// <returns></returns>
        public static IQueryable<Answer> WhereRaspunsContains(this IQueryable<Answer> source, IList<int> contains)
        {
            var ors = contains
                .Aggregate<int, Expression<Func<Answer, bool>>>(null, (expression, id) =>
                    expression == null
                        ? (a => a.OptionAnswered.QuestionId == id)
                        : expression.Or(a => a.OptionAnswered.QuestionId == id));

            return source.Where(ors);
        }
    }
}