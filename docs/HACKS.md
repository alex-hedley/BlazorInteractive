# Hacks

Overriding an internal method (reflection, emitting)
- https://stackoverflow.com/a/67926929/2895831

Overriding Sealed Methods in C#
- https://www.infoq.com/articles/overriding-sealed-methods-c-sharp/

---

`public override String Location`

`private static extern void GetLocation(RuntimeAssembly assembly, StringHandleOnStack retString);`

- https://github.com/microsoft/referencesource/blob/master/mscorlib/system/reflection/assembly.cs#L2372-L2389

---

Is it possible to access backing fields behind auto-implemented properties?
- https://stackoverflow.com/questions/8817070/is-it-possible-to-access-backing-fields-behind-auto-implemented-properties/14210097#14210097
  - https://stackoverflow.com/a/14210097/2895831

```csharp
private string _getBackingFieldName(string propertyName)
{
    return string.Format("<{0}>k__BackingField", propertyName);
}
```

```csharp
private FieldInfo _getBackingField(object obj, string propertyName)
{
    return obj.GetType().GetField(_getBackingFieldName(propertyName), BindingFlags.Instance | BindingFlags.NonPublic);
}
```

---
