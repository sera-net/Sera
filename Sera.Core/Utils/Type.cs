using System;
using System.Globalization;
using System.Reflection;

namespace Sera.Utils;

public struct Type<T> : IEquatable<Type<T>>, IEquatable<Type>, IReflect
{
    public Type TypeOf => typeof(T);

    public static implicit operator Type(Type<T> _) => typeof(T);

    public override string ToString() => typeof(T).ToString();

    #region Equals

    public bool Equals(Type<T> other) => true;

    public bool Equals(Type? other) => other != null && typeof(T) == other;

    public override bool Equals(object? obj) => obj is Type<T> other && Equals(other);

    public override int GetHashCode() => typeof(T).GetHashCode();

    public static bool operator ==(Type<T> left, Type<T> right) => left.Equals(right);

    public static bool operator !=(Type<T> left, Type<T> right) => !left.Equals(right);

    #endregion

    #region IReflect

    public FieldInfo? GetField(string name, BindingFlags bindingAttr)
        => typeof(T).GetField(name, bindingAttr);

    public FieldInfo[] GetFields(BindingFlags bindingAttr)
        => typeof(T).GetFields(bindingAttr);

    public MemberInfo[] GetMember(string name, BindingFlags bindingAttr)
        => typeof(T).GetMember(name, bindingAttr);

    public MemberInfo[] GetMembers(BindingFlags bindingAttr)
        => typeof(T).GetMembers(bindingAttr);

    public MethodInfo? GetMethod(string name, BindingFlags bindingAttr)
        => typeof(T).GetMethod(name, bindingAttr);

    public MethodInfo? GetMethod(string name, BindingFlags bindingAttr, Binder? binder, Type[] types,
        ParameterModifier[]? modifiers) =>
        typeof(T).GetMethod(name, bindingAttr, binder, types, modifiers);

    public MethodInfo[] GetMethods(BindingFlags bindingAttr)
        => typeof(T).GetMethods(bindingAttr);

    public PropertyInfo[] GetProperties(BindingFlags bindingAttr)
        => typeof(T).GetProperties(bindingAttr);

    public PropertyInfo? GetProperty(string name, BindingFlags bindingAttr)
        => typeof(T).GetProperty(name, bindingAttr);

    public PropertyInfo? GetProperty(string name, BindingFlags bindingAttr, Binder? binder, Type? returnType,
        Type[] types,
        ParameterModifier[]? modifiers) =>
        typeof(T).GetProperty(name, bindingAttr, binder, returnType, types, modifiers);

    public object? InvokeMember(string name, BindingFlags invokeAttr, Binder? binder, object? target, object?[]? args,
        ParameterModifier[]? modifiers, CultureInfo? culture, string[]? namedParameters) =>
        typeof(T).InvokeMember(name, invokeAttr, binder, target, args, modifiers, culture, namedParameters);

    public Type UnderlyingSystemType => typeof(T).UnderlyingSystemType;

    #endregion
}
