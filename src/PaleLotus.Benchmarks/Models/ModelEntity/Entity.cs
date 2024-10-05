using System.Collections;
using System.Diagnostics;
using System.Dynamic;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using PaleLotus.Benchmarks.LinkModels;

namespace PaleLotus.Benchmarks.Models.ModelEntity;

public class Entity : DynamicObject, IXmlSerializable, IDictionary<string, object>
{
    //todo: check if its ok or full ns?
    private const string Root = "ModelEntity";
    private readonly IDictionary<string, object?> _expando = new ExpandoObject();

    public override bool TryGetMember(GetMemberBinder binder, out object? result)
    {
        if (!_expando.TryGetValue(binder.Name, out var value))
            return base.TryGetMember(binder, out result);
        result = value;
        return true;
    }

    public override bool TrySetMember(SetMemberBinder binder, object? value)
    {
        Debug.Assert(value != null, nameof(value) + " != null");
        _expando[binder.Name] = value;
        return true;
    }

    public XmlSchema GetSchema()
    {
        throw new NotImplementedException();
    }

    public void ReadXml(XmlReader reader)
    {
        reader.ReadStartElement(Root);

        while (!reader.Name.Equals(Root))
        {
            var name = reader.Name;

            reader.MoveToAttribute("type");
            var typeContent = reader.ReadContentAsString();
            var underlyingType = Type.GetType(typeContent);
            reader.MoveToContent();
            
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
            Debug.Assert(underlyingType != null, nameof(underlyingType) + " != null");
            _expando[name] = reader.ReadElementContentAs(underlyingType, null);
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
        }
    }

    public void WriteXml(XmlWriter writer)
    {
        foreach (var key in _expando.Keys)
        {
            var value = _expando[key];
            WriteLinksToXml(key, value, writer);
        }
    }

    private static void WriteLinksToXml(string key, object? value, XmlWriter writer)
    {
        writer.WriteStartElement(key);
        if (value.GetType() != typeof(List<Link>))
        {
            writer.WriteString(value?.ToString());
            writer.WriteEndElement();
            return;
        }

        foreach (var val in (value as List<Link>)!)
        {
            writer.WriteStartElement(nameof(Link));
            WriteLinksToXml(nameof(val.Href), val.Href, writer);
            WriteLinksToXml(nameof(val.Method), val.Method, writer);
            WriteLinksToXml(nameof(val.Rel), val.Rel, writer);
            writer.WriteEndElement();
        }
    }

    public void Add(string key, object? value) => _expando.Add(key, value);

    public bool ContainsKey(string key) => _expando.ContainsKey(key);

    public ICollection<string> Keys => _expando.Keys;

    public bool Remove(string key) => _expando.Remove(key);

    public bool TryGetValue(string key, out object value) => _expando.TryGetValue(key, out value!);

    public ICollection<object> Values => _expando.Values!;

    public object this[string key]
    {
        get => _expando[key]!;
        set => _expando[key] = value;
    }

    public void Add(KeyValuePair<string, object> item) => _expando.Add(item!);

    public void Clear() => _expando.Clear();

    public bool Contains(KeyValuePair<string, object> item) => _expando.Contains(item!);

    public void CopyTo(KeyValuePair<string, object>[] array, int arrayIndex) => _expando.CopyTo(array!, arrayIndex);

    public int Count => _expando.Count;

    public bool IsReadOnly => _expando.IsReadOnly;

    public bool Remove(KeyValuePair<string, object> item) => _expando.Remove(item!);

    public IEnumerator<KeyValuePair<string, object>> GetEnumerator() => _expando.GetEnumerator()!;

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}