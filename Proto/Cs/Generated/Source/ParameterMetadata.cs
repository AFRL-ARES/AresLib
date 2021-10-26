// <auto-generated>
//     Generated by the protocol buffer compiler.  DO NOT EDIT!
//     source: ParameterMetadata.proto
// </auto-generated>
#pragma warning disable 1591, 0612, 3021
#region Designer generated code

using pb = global::Google.Protobuf;
using pbc = global::Google.Protobuf.Collections;
using pbr = global::Google.Protobuf.Reflection;
using scg = global::System.Collections.Generic;
namespace Ares.Core {

  /// <summary>Holder for reflection information generated from ParameterMetadata.proto</summary>
  public static partial class ParameterMetadataReflection {

    #region Descriptor
    /// <summary>File descriptor for ParameterMetadata.proto</summary>
    public static pbr::FileDescriptor Descriptor {
      get { return descriptor; }
    }
    private static pbr::FileDescriptor descriptor;

    static ParameterMetadataReflection() {
      byte[] descriptorData = global::System.Convert.FromBase64String(
          string.Concat(
            "ChdQYXJhbWV0ZXJNZXRhZGF0YS5wcm90bxIJYXJlcy5jb3JlGgxMaW1pdHMu",
            "cHJvdG8iVwoRUGFyYW1ldGVyTWV0YWRhdGESDAoETmFtZRgBIAEoCRIMCgRV",
            "bml0GAIgASgJEiYKC0NvbnN0cmFpbnRzGAMgAygLMhEuYXJlcy5jb3JlLkxp",
            "bWl0c2IGcHJvdG8z"));
      descriptor = pbr::FileDescriptor.FromGeneratedCode(descriptorData,
          new pbr::FileDescriptor[] { global::Ares.Core.LimitsReflection.Descriptor, },
          new pbr::GeneratedClrTypeInfo(null, null, new pbr::GeneratedClrTypeInfo[] {
            new pbr::GeneratedClrTypeInfo(typeof(global::Ares.Core.ParameterMetadata), global::Ares.Core.ParameterMetadata.Parser, new[]{ "Name", "Unit", "Constraints" }, null, null, null, null)
          }));
    }
    #endregion

  }
  #region Messages
  public sealed partial class ParameterMetadata : pb::IMessage<ParameterMetadata> {
    private static readonly pb::MessageParser<ParameterMetadata> _parser = new pb::MessageParser<ParameterMetadata>(() => new ParameterMetadata());
    private pb::UnknownFieldSet _unknownFields;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pb::MessageParser<ParameterMetadata> Parser { get { return _parser; } }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pbr::MessageDescriptor Descriptor {
      get { return global::Ares.Core.ParameterMetadataReflection.Descriptor.MessageTypes[0]; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    pbr::MessageDescriptor pb::IMessage.Descriptor {
      get { return Descriptor; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public ParameterMetadata() {
      OnConstruction();
    }

    partial void OnConstruction();

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public ParameterMetadata(ParameterMetadata other) : this() {
      name_ = other.name_;
      unit_ = other.unit_;
      constraints_ = other.constraints_.Clone();
      _unknownFields = pb::UnknownFieldSet.Clone(other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public ParameterMetadata Clone() {
      return new ParameterMetadata(this);
    }

    /// <summary>Field number for the "Name" field.</summary>
    public const int NameFieldNumber = 1;
    private string name_ = "";
    /// <summary>
    /// Name descriptor of the argument/parameter
    /// </summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public string Name {
      get { return name_; }
      set {
        name_ = pb::ProtoPreconditions.CheckNotNull(value, "value");
      }
    }

    /// <summary>Field number for the "Unit" field.</summary>
    public const int UnitFieldNumber = 2;
    private string unit_ = "";
    /// <summary>
    /// Unit descriptor, if applicable, of the argument/parameter
    /// </summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public string Unit {
      get { return unit_; }
      set {
        unit_ = pb::ProtoPreconditions.CheckNotNull(value, "value");
      }
    }

    /// <summary>Field number for the "Constraints" field.</summary>
    public const int ConstraintsFieldNumber = 3;
    private static readonly pb::FieldCodec<global::Ares.Core.Limits> _repeated_constraints_codec
        = pb::FieldCodec.ForMessage(26, global::Ares.Core.Limits.Parser);
    private readonly pbc::RepeatedField<global::Ares.Core.Limits> constraints_ = new pbc::RepeatedField<global::Ares.Core.Limits>();
    /// <summary>
    /// Collection of limits for the argument/parameter formatted to be easily converted into planner requests.
    /// </summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public pbc::RepeatedField<global::Ares.Core.Limits> Constraints {
      get { return constraints_; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override bool Equals(object other) {
      return Equals(other as ParameterMetadata);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public bool Equals(ParameterMetadata other) {
      if (ReferenceEquals(other, null)) {
        return false;
      }
      if (ReferenceEquals(other, this)) {
        return true;
      }
      if (Name != other.Name) return false;
      if (Unit != other.Unit) return false;
      if(!constraints_.Equals(other.constraints_)) return false;
      return Equals(_unknownFields, other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override int GetHashCode() {
      int hash = 1;
      if (Name.Length != 0) hash ^= Name.GetHashCode();
      if (Unit.Length != 0) hash ^= Unit.GetHashCode();
      hash ^= constraints_.GetHashCode();
      if (_unknownFields != null) {
        hash ^= _unknownFields.GetHashCode();
      }
      return hash;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override string ToString() {
      return pb::JsonFormatter.ToDiagnosticString(this);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void WriteTo(pb::CodedOutputStream output) {
      if (Name.Length != 0) {
        output.WriteRawTag(10);
        output.WriteString(Name);
      }
      if (Unit.Length != 0) {
        output.WriteRawTag(18);
        output.WriteString(Unit);
      }
      constraints_.WriteTo(output, _repeated_constraints_codec);
      if (_unknownFields != null) {
        _unknownFields.WriteTo(output);
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public int CalculateSize() {
      int size = 0;
      if (Name.Length != 0) {
        size += 1 + pb::CodedOutputStream.ComputeStringSize(Name);
      }
      if (Unit.Length != 0) {
        size += 1 + pb::CodedOutputStream.ComputeStringSize(Unit);
      }
      size += constraints_.CalculateSize(_repeated_constraints_codec);
      if (_unknownFields != null) {
        size += _unknownFields.CalculateSize();
      }
      return size;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void MergeFrom(ParameterMetadata other) {
      if (other == null) {
        return;
      }
      if (other.Name.Length != 0) {
        Name = other.Name;
      }
      if (other.Unit.Length != 0) {
        Unit = other.Unit;
      }
      constraints_.Add(other.constraints_);
      _unknownFields = pb::UnknownFieldSet.MergeFrom(_unknownFields, other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void MergeFrom(pb::CodedInputStream input) {
      uint tag;
      while ((tag = input.ReadTag()) != 0) {
        switch(tag) {
          default:
            _unknownFields = pb::UnknownFieldSet.MergeFieldFrom(_unknownFields, input);
            break;
          case 10: {
            Name = input.ReadString();
            break;
          }
          case 18: {
            Unit = input.ReadString();
            break;
          }
          case 26: {
            constraints_.AddEntriesFrom(input, _repeated_constraints_codec);
            break;
          }
        }
      }
    }

  }

  #endregion

}

#endregion Designer generated code