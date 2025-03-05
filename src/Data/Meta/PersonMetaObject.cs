namespace DynamicTypeLinqQuery.Data.Meta;

using DynamicTypeLinqQuery.Data;
using System;
using System.Dynamic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

internal class PersonMetaObject : DynamicMetaObject {
    private static readonly TypeInfo _typeInfo = typeof(Person).GetTypeInfo();

    public PersonMetaObject(Expression parameter, Person value)
        : base(parameter, BindingRestrictions.Empty, value) { }

    public override DynamicMetaObject BindGetMember(GetMemberBinder binder) {
        Expression self = GetSelfExpression();

        string propertyName = binder.Name;

        return GetDynamicMetaObject(self, propertyName);
    }

    public override DynamicMetaObject BindGetIndex(GetIndexBinder binder, DynamicMetaObject[] indexes) {
        Expression self = GetSelfExpression();

        string propertyName = indexes.First().Value.ToString();

        return GetDynamicMetaObject(self, propertyName);
    }

    private DynamicMetaObject GetDynamicMetaObject(Expression self, string propertyName) {
        switch (propertyName) {
            // Dynamic value computed in runtime
            case "FullName": {
                // Get FirstName property
                Expression firstName = Expression.Property(self, nameof(Person.FirstName));
                // Get LastName property
                Expression lastName = Expression.Property(self, nameof(Person.LastName));
                // Create constant containing space character
                Expression space = Expression.Constant(" ");
                // Create an array of strings containing first name and last name
                Expression values = Expression.NewArrayInit(typeof(string), firstName, lastName);
                // Call String.Join method with values to join and a delimiter
                Expression fullName = Expression.Call(null, typeof(string).GetMethod(nameof(String.Join), [typeof(string), typeof(string[])]), space, values);
                // Create and return dynamic object metadata
                return new DynamicMetaObject(Expression.Convert(fullName, typeof(object)), BindingRestrictions.GetTypeRestriction(Expression, LimitType));
            }
            // Defined method invoked
            case "Age": {
                // Get method info using type reflection
                MethodInfo getAge = _typeInfo.GetMethod(nameof(Person.GetAge));
                // Call reflected method
                Expression age = Expression.Call(self, getAge);
                // Create and return dynamic object metadata
                return new DynamicMetaObject(Expression.Convert(age, typeof(object)), BindingRestrictions.GetTypeRestriction(Expression, LimitType));
            }
            // 
            default: {
                // If property is part of the Person type create an accessor and return dynamic object metadata
                if (_typeInfo.GetMembers().Any(m => m.Name == propertyName)) {
                    return new DynamicMetaObject(Expression.Convert(Expression.Property(self, propertyName), typeof(object)), BindingRestrictions.GetTypeRestriction(Expression, LimitType));
                }

                // Create default object
                Expression @default = Expression.Default(typeof(object));
                // Create and return dynamic object metadata
                return new DynamicMetaObject(@default, BindingRestrictions.GetTypeRestriction(Expression, LimitType));
            }
        }
    }
    private UnaryExpression GetSelfExpression() {
        return Expression.Convert(Expression, LimitType);
    }
}