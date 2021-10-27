// <auto-generated>
//     Generated by the protocol buffer compiler.  DO NOT EDIT!
//     source: CompletedExperiment.proto
// </auto-generated>
#pragma warning disable 1591, 0612, 3021
#region Designer generated code

using pb = global::Google.Protobuf;
using pbc = global::Google.Protobuf.Collections;
using pbr = global::Google.Protobuf.Reflection;
using scg = global::System.Collections.Generic;
namespace Ares.Core {

  /// <summary>Holder for reflection information generated from CompletedExperiment.proto</summary>
  public static partial class CompletedExperimentReflection {

    #region Descriptor
    /// <summary>File descriptor for CompletedExperiment.proto</summary>
    public static pbr::FileDescriptor Descriptor {
      get { return descriptor; }
    }
    private static pbr::FileDescriptor descriptor;

    static CompletedExperimentReflection() {
      byte[] descriptorData = global::System.Convert.FromBase64String(
          string.Concat(
            "ChlDb21wbGV0ZWRFeHBlcmltZW50LnByb3RvEglhcmVzLmNvcmUaGEV4cGVy",
            "aW1lbnRUZW1wbGF0ZS5wcm90byJuChNDb21wbGV0ZWRFeHBlcmltZW50Ei8K",
            "CFRlbXBsYXRlGAEgASgLMh0uYXJlcy5jb3JlLkV4cGVyaW1lbnRUZW1wbGF0",
            "ZRIWCg5TZXJpYWxpemVkRGF0YRgCIAEoDBIOCgZGb3JtYXQYAyABKAliBnBy",
            "b3RvMw=="));
      descriptor = pbr::FileDescriptor.FromGeneratedCode(descriptorData,
          new pbr::FileDescriptor[] { global::Ares.Core.ExperimentTemplateReflection.Descriptor, },
          new pbr::GeneratedClrTypeInfo(null, null, new pbr::GeneratedClrTypeInfo[] {
            new pbr::GeneratedClrTypeInfo(typeof(global::Ares.Core.CompletedExperiment), global::Ares.Core.CompletedExperiment.Parser, new[]{ "Template", "SerializedData", "Format" }, null, null, null, null)
          }));
    }
    #endregion

  }
  #region Messages
  public sealed partial class CompletedExperiment : pb::IMessage<CompletedExperiment> {
    private static readonly pb::MessageParser<CompletedExperiment> _parser = new pb::MessageParser<CompletedExperiment>(() => new CompletedExperiment());
    private pb::UnknownFieldSet _unknownFields;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pb::MessageParser<CompletedExperiment> Parser { get { return _parser; } }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pbr::MessageDescriptor Descriptor {
      get { return global::Ares.Core.CompletedExperimentReflection.Descriptor.MessageTypes[0]; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    pbr::MessageDescriptor pb::IMessage.Descriptor {
      get { return Descriptor; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public CompletedExperiment() {
      OnConstruction();
    }

    partial void OnConstruction();

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public CompletedExperiment(CompletedExperiment other) : this() {
      template_ = other.template_ != null ? other.template_.Clone() : null;
      serializedData_ = other.serializedData_;
      format_ = other.format_;
      _unknownFields = pb::UnknownFieldSet.Clone(other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public CompletedExperiment Clone() {
      return new CompletedExperiment(this);
    }

    /// <summary>Field number for the "Template" field.</summary>
    public const int TemplateFieldNumber = 1;
    private global::Ares.Core.ExperimentTemplate template_;
    /// <summary>
    /// Template used to generate the executable experiment providing the results
    /// </summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public global::Ares.Core.ExperimentTemplate Template {
      get { return template_; }
      set {
        template_ = value;
      }
    }

    /// <summary>Field number for the "SerializedData" field.</summary>
    public const int SerializedDataFieldNumber = 2;
    private pb::ByteString serializedData_ = pb::ByteString.Empty;
    /// <summary>
    /// Serialized data representation of the experiment output
    /// </summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public pb::ByteString SerializedData {
      get { return serializedData_; }
      set {
        serializedData_ = pb::ProtoPreconditions.CheckNotNull(value, "value");
      }
    }

    /// <summary>Field number for the "Format" field.</summary>
    public const int FormatFieldNumber = 3;
    private string format_ = "";
    /// <summary>
    /// Text indicating the format of serialized data, hopefully useful for deserializing.
    /// </summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public string Format {
      get { return format_; }
      set {
        format_ = pb::ProtoPreconditions.CheckNotNull(value, "value");
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override bool Equals(object other) {
      return Equals(other as CompletedExperiment);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public bool Equals(CompletedExperiment other) {
      if (ReferenceEquals(other, null)) {
        return false;
      }
      if (ReferenceEquals(other, this)) {
        return true;
      }
      if (!object.Equals(Template, other.Template)) return false;
      if (SerializedData != other.SerializedData) return false;
      if (Format != other.Format) return false;
      return Equals(_unknownFields, other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override int GetHashCode() {
      int hash = 1;
      if (template_ != null) hash ^= Template.GetHashCode();
      if (SerializedData.Length != 0) hash ^= SerializedData.GetHashCode();
      if (Format.Length != 0) hash ^= Format.GetHashCode();
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
      if (template_ != null) {
        output.WriteRawTag(10);
        output.WriteMessage(Template);
      }
      if (SerializedData.Length != 0) {
        output.WriteRawTag(18);
        output.WriteBytes(SerializedData);
      }
      if (Format.Length != 0) {
        output.WriteRawTag(26);
        output.WriteString(Format);
      }
      if (_unknownFields != null) {
        _unknownFields.WriteTo(output);
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public int CalculateSize() {
      int size = 0;
      if (template_ != null) {
        size += 1 + pb::CodedOutputStream.ComputeMessageSize(Template);
      }
      if (SerializedData.Length != 0) {
        size += 1 + pb::CodedOutputStream.ComputeBytesSize(SerializedData);
      }
      if (Format.Length != 0) {
        size += 1 + pb::CodedOutputStream.ComputeStringSize(Format);
      }
      if (_unknownFields != null) {
        size += _unknownFields.CalculateSize();
      }
      return size;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void MergeFrom(CompletedExperiment other) {
      if (other == null) {
        return;
      }
      if (other.template_ != null) {
        if (template_ == null) {
          Template = new global::Ares.Core.ExperimentTemplate();
        }
        Template.MergeFrom(other.Template);
      }
      if (other.SerializedData.Length != 0) {
        SerializedData = other.SerializedData;
      }
      if (other.Format.Length != 0) {
        Format = other.Format;
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
            if (template_ == null) {
              Template = new global::Ares.Core.ExperimentTemplate();
            }
            input.ReadMessage(Template);
            break;
          }
          case 18: {
            SerializedData = input.ReadBytes();
            break;
          }
          case 26: {
            Format = input.ReadString();
            break;
          }
        }
      }
    }

  }

  #endregion

}

#endregion Designer generated code
