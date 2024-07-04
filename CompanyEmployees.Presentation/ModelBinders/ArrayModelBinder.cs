using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CompanyEmployees.Presentation.ModelBinders
{
    public class ArrayModelBinder : IModelBinder
    {
        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            //check if our parameter is enumerable type
            if (!bindingContext.ModelMetadata.IsEnumerableType)
            {
                bindingContext.Result = ModelBindingResult.Failed();
                return Task.CompletedTask;
            }

            // extract the value
            var providedValue = bindingContext.ValueProvider
                    .GetValue(bindingContext.ModelName).ToString();
            
            if (string.IsNullOrEmpty(providedValue))
            {
                bindingContext.Result = ModelBindingResult.Success(null);
                return Task.CompletedTask;
            }

            //for partly generic binder
            //get type in enumerable
            var genericType = bindingContext.ModelType.GetTypeInfo().GenericTypeArguments[0];
            //converter for getted type
            var converter = TypeDescriptor.GetConverter(genericType);

            var objectArray = providedValue.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(x => converter.ConvertFromString(x.Trim())).ToArray();

            var guidArray = Array.CreateInstance(genericType, objectArray.Length);
            
            objectArray.CopyTo(guidArray, 0);
            bindingContext.Model = guidArray;
            bindingContext.Result = ModelBindingResult.Success(bindingContext.Model);
            
            return Task.CompletedTask;
        }
    }
}
