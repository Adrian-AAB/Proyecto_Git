﻿using Peasy.Extensions;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using Orders.com.BLL.Domain;
using Peasy;

namespace Orders.com.BLL.Rules
{
    public class CannotEditPropertyRule : RuleBase
    {
        private string _propertyName;

        public CannotEditPropertyRule(string propertyName)
        {
            _propertyName = propertyName;
        }

        protected override void OnValidate()
        {
            Invalidate(string.Format("{0} cannot be changed.", _propertyName));
        }

        protected override Task OnValidateAsync()
        {
            return Task.Run(() => OnValidate());
        }
    }

    public class CannotEditPropertyRule<T, D> : RuleBase where T : DomainBase
    {
        private T _original;
        private T _changed;
        private Expression<Func<D>> _propertyExpression;

        public CannotEditPropertyRule(T original, T changed, Expression<Func<D>> propertyExpression)
        {
            _original = original;
            _changed = changed;
            _propertyExpression = propertyExpression;
        }

        protected override void OnValidate()
        {
            var propertyName = ExpressionExtensions.ExtractPropertyName(_propertyExpression);
            var originalType = typeof(T);
            var property = originalType.GetTypeInfo().DeclaredProperties.First(p => p.Name == propertyName);
            var originalValue = property.GetValue(_original);
            var changedValue = property.GetValue(_changed);
            if (originalValue != changedValue)
            {
                Invalidate(string.Format("{0} cannot be changed.", propertyName));
            }
        }
    }
}
