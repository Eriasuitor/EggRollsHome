using System;
using System.Collections.Generic;
using FluentValidation.Results;
using Newegg.API.Client;
using Newegg.MIS.API.Utilities.Entities;
using Newegg.MIS.API.Utilities.Exceptions;
using ValidationErrorResponse = Newegg.MIS.API.Utilities.Entities.ValidationErrorResponse;

namespace Newegg.MIS.API.Utilities.Extensions
{
    public static class GeneralResponseExtension
    {
        public static void CaptureValidationResult(this GeneralResponse response,
            ValidationResult validationResult)
        {
            if (validationResult.IsValid) return;
            response.Succeeded = false;

            if (response.ValidationErrors == null)
            {
                response.ValidationErrors = new List<ValidationErrorResponse>();
            }

            if (validationResult.Errors == null) return;

            foreach (var error in validationResult.Errors)
            {
                response.ValidationErrors.Add(new ValidationErrorResponse
                {
                    AttemptedValue = error.AttemptedValue,
                    CustomState = error.CustomState,
                    ErrorMessage = error.ErrorMessage,
                    PropertyName = error.PropertyName
                });
            }
        }

        public static void CaptureException(this GeneralResponse response, Exception ex)
        {
            response.Succeeded = false;

            if (null == response.Errors)
            {
                response.Errors = new List<Error>();
            }

            var serviceException = ex as WebServiceException;
            if (serviceException != null)
            {
                if (serviceException.HasValidationError)
                {
                    HandleAPIValidationException(response, serviceException);
                    return;
                }
            }

            CollectException(response, ex);
        }

        private static void CollectException(GeneralResponse response, Exception exception)
        {
            if (null == exception) return;
            var businessException = exception as BusinessException;
            if (businessException != null)
            {
                response.Errors.Add(new Error
                {
                    Code = businessException.Code,
                    Message = businessException.Message
                });

                CollectException(response, exception.InnerException);
                return;
            }

            response.Errors.Add(
                new Error
                {
                    Code = "ERROR",
#if DEBUG
                    Message = exception.Message + Environment.NewLine + exception.StackTrace
#else
                    Message = exception.Message
#endif
                }
                );
        }

        private static void HandleAPIValidationException(
            GeneralResponse response,
            WebServiceException exception)
        {
            if (null == exception.ResponseDto) return;
            if (null == response.Errors) response.Errors = new List<Error>();

            if (null == exception.ResponseDto.ValidationErrors ||
                exception.ResponseDto.ValidationErrors.Count == 0)
            {
                response.Errors.Add(new Error
                {
                    Code = exception.ResponseDto.ResponseCode,
                    Message = exception.ResponseDto.Message
                });

                return;
            }

            foreach (var validationErrorResponse in exception.ResponseDto.ValidationErrors)
            {
                response.Errors.Add(new Error
                {
                    Code = validationErrorResponse.PropertyName,
                    Message = validationErrorResponse.ErrorMessage
                });
            }
        }
    }
}
