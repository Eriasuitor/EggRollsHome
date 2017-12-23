using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newegg.MIS.API.Utilities.Entities;
using Newegg.API.Validation;
using FluentValidation;

namespace Newegg.MIS.API.Utilities.Validation
{
    public class PagingInfoValidator : CustomerValidator<PagingInfo>
    {
        public PagingInfoValidator()
        {
            RuleSet(ApplyTo.Post, () =>
            {
                RuleFor(dto => dto.PageSize, true)
                    .NotNull()
                    .Must(PageSizeIsValid)
                    .WithMessage("Page size must be provided and must be a positive integer.");

                RuleFor(dto => dto, true)
                    .Must(IndexesIsValid)
                    .OverridePropertyName("StartPageIndex / EndPageIndex")
                    .WithMessage("One of StartPageIndex and EndPageIndex must be provided at least," +
                    " to return a specific page, you can set StartPageIndex equal to EndPageIndex, " +
                    "and StartPageIndex must be less than or equal to EndPageIndex.");
            });

        }

        public static bool IndexesIsValid(PagingInfo pagingInfo)
        {
            if (pagingInfo.StartPageIndex.HasValue && pagingInfo.StartPageIndex.Value < 0) return false;
            if (pagingInfo.EndPageIndex.HasValue && pagingInfo.EndPageIndex.Value < 0) return false;

            if (!pagingInfo.StartPageIndex.HasValue && !pagingInfo.EndPageIndex.HasValue) return false;

            if (pagingInfo.StartPageIndex.HasValue && pagingInfo.EndPageIndex.HasValue)
            {
                if (pagingInfo.StartPageIndex.Value > pagingInfo.EndPageIndex.Value) return false;
            }

            return true;
        }

        public const int MaximumPageSize = 2000;

        public static bool PageSizeIsValid(int? pageSize)
        {
            if (!pageSize.HasValue) return false;
            if (pageSize.Value <= 0) return false;

            if (pageSize.Value > MaximumPageSize) return false;

            return true;
        }
    }
}
