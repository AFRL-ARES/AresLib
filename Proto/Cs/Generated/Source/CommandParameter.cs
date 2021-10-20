// <auto-generated>
//     Generated by the protocol buffer compiler.  DO NOT EDIT!
//     source: CommandParameter.proto
// </auto-generated>
#pragma warning disable 1591, 0612, 3021
#region Designer generated code

using pb = global::Google.Protobuf;
using pbc = global::Google.Protobuf.Collections;
using pbr = global::Google.Protobuf.Reflection;
using scg = global::System.Collections.Generic;
namespace Ares.Core {

  /// <summary>Holder for reflection information generated from CommandParameter.proto</summary>
  public static partial class CommandParameterReflection {

    #region Descriptor
    /// <summary>File descriptor for CommandParameter.proto</summary>
    public static pbr::FileDescriptor Descriptor {
      get { return descriptor; }
    }
    private static pbr::FileDescriptor descriptor;

    static CommandParameterReflection() {
      byte[] descriptorData = global::System.Convert.FromBase64String(
          string.Concat(
            "ChZDb21tYW5kUGFyYW1ldGVyLnByb3RvEglhcmVzLmNvcmUaHkNvbW1hbmRQ",
            "YXJhbWV0ZXJNZXRhZGF0YS5wcm90byJYChBDb21tYW5kUGFyYW1ldGVyEjUK",
            "CE1ldGFkYXRhGAEgASgLMiMuYXJlcy5jb3JlLkNvbW1hbmRQYXJhbWV0ZXJN",
            "ZXRhZGF0YRINCgVWYWx1ZRgCIAEoAmIGcHJvdG8z"));
      descriptor = pbr::FileDescriptor.FromGeneratedCode(descriptorData,
          new pbr::FileDescriptor[] { global::Ares.Core.CommandParameterMetadataReflection.Descriptor, },
          new pbr::GeneratedClrTypeInfo(null, null, new pbr::GeneratedClrTypeInfo[] {
            new pbr::GeneratedClrTypeInfo(typeof(global::Ares.Core.CommandParameter), global::Ares.Core.CommandParameter.Parser, new[]{ "Metadata", "Value" }, null, null, null, null)
          }));
    }
    #endregion

  }
  #region Messages
  public sealed partial class CommandParameter : pb::IMessage<CommandParameter> {
    private static readonly pb::MessageParser<CommandParameter> _parser = new pb::MessageParser<CommandParameter>(() => new CommandParameter());
    private pb::UnknownFieldSet _unknownFields;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pb::MessageParser<CommandParameter> Parser { get { return _parser; } }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pbr::MessageDescriptor Descriptor {
      get { return global::Ares.Core.CommandParameterReflection.Descriptor.MessageTypes[0]; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    pbr::MessageDescriptor pb::IMessage.Descriptor {
      get { return Descriptor; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public CommandParameter() {
      OnConstruction();
    }

    partial void OnConstruction();

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public CommandParameter(CommandParameter other) : this() {
      metadata_ = other.metadata_ != null ? other.metadata_.Clone() : null;
      value_ = other.value_;
      _unknownFields = pb::UnknownFieldSet.Clone(other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public CommandParameter Clone() {
      return new CommandParameter(this);
    }

    /// <summary>Field number for the "Metadata" field.</summary>
    public const int MetadataFieldNumber = 1;
    private global::Ares.Core.CommandParameterMetadata metadata_;
    /// <summary>
    /// Non-volatile information about the command parameter
    /// </summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public global::Ares.Core.CommandParameterMetadata Metadata {
      get { return metadata_; }
      set {
        metadata_ = value;
      }
    }

    /// <summary>Field number for the "Value" field.</summary>
    public const int ValueFieldNumber = 2;
    private float value_;
    /// <summary>
    /// Value of the parameter, represented as a floating point number for planner compatability
    /// </summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public float Value {
      get { return value_; }
      set {
        value_ = value;
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override bool Equals(object other) {
      return Equals(other as CommandParameter);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public bool Equals(CommandParameter other) {
      if (ReferenceEquals(other, null)) {
        return false;
      }
      if (ReferenceEquals(other, this)) {
        return true;
      }
      if (!object.Equals(Metadata, other.Metadata)) return false;
      if (!pbc::ProtobufEqualityComparers.BitwiseSingleEqualityComparer.Equals(Value, other.Value)) return false;
      return Equals(_unknownFields, other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override int GetHashCode() {
      int hash = 1;
      if (metadata_ != null) hash ^= Metadata.GetHashCode();
      if (Value != 0F) hash ^= pbc::ProtobufEqualityComparers.BitwiseSingleEqualityComparer.GetHashCode(Value);
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
      if (metadata_ != null) {
        output.WriteRawTag(10);
        output.WriteMessage(Metadata);
      }
      if (Value != 0F) {
        output.WriteRawTag(21);
        output.WriteFloat(Value);
      }
      if (_unknownFields != null) {
        _unknownFields.WriteTo(output);
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public int CalculateSize() {
      int size = 0;
      if (metadata_ != null) {
        size += 1 + pb::CodedOutputStream.ComputeMessageSize(Metadata);
      }
      if (Value != 0F) {
        size += 1 + 4;
      }
      if (_unknownFields != null) {
        size += _unknownFields.CalculateSize();
      }
      return size;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void MergeFrom(CommandParameter other) {
      if (other == null) {
        return;
      }
      if (other.metadata_ != null) {
        if (metadata_ == null) {
          Metadata = new global::Ares.Core.CommandParameterMetadata();
        }
        Metadata.MergeFrom(other.Metadata);
      }
      if (other.Value != 0F) {
        Value = other.Value;
      }
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
            if (metadata_ == null) {
              Metadata = new global::Ares.Core.CommandParameterMetadata();
            }
            input.ReadMessage(Metadata);
            break;
          }
          case 21: {
            Value = input.ReadFloat();
            break;
          }
        }
      }
    }

  }

  #endregion

}

#endregion Designer generated code
