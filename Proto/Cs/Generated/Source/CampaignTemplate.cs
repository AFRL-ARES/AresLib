// <auto-generated>
//     Generated by the protocol buffer compiler.  DO NOT EDIT!
//     source: CampaignTemplate.proto
// </auto-generated>
#pragma warning disable 1591, 0612, 3021
#region Designer generated code

using pb = global::Google.Protobuf;
using pbc = global::Google.Protobuf.Collections;
using pbr = global::Google.Protobuf.Reflection;
using scg = global::System.Collections.Generic;
namespace Ares.Core {

  /// <summary>Holder for reflection information generated from CampaignTemplate.proto</summary>
  public static partial class CampaignTemplateReflection {

    #region Descriptor
    /// <summary>File descriptor for CampaignTemplate.proto</summary>
    public static pbr::FileDescriptor Descriptor {
      get { return descriptor; }
    }
    private static pbr::FileDescriptor descriptor;

    static CampaignTemplateReflection() {
      byte[] descriptorData = global::System.Convert.FromBase64String(
          string.Concat(
            "ChZDYW1wYWlnblRlbXBsYXRlLnByb3RvEglhcmVzLmNvcmUaGEV4cGVyaW1l",
            "bnRUZW1wbGF0ZS5wcm90byJcChBDYW1wYWlnblRlbXBsYXRlEjoKE0V4cGVy",
            "aW1lbnRUZW1wbGF0ZXMYASADKAsyHS5hcmVzLmNvcmUuRXhwZXJpbWVudFRl",
            "bXBsYXRlEgwKBE5hbWUYAiABKAliBnByb3RvMw=="));
      descriptor = pbr::FileDescriptor.FromGeneratedCode(descriptorData,
          new pbr::FileDescriptor[] { global::Ares.Core.ExperimentTemplateReflection.Descriptor, },
          new pbr::GeneratedClrTypeInfo(null, null, new pbr::GeneratedClrTypeInfo[] {
            new pbr::GeneratedClrTypeInfo(typeof(global::Ares.Core.CampaignTemplate), global::Ares.Core.CampaignTemplate.Parser, new[]{ "ExperimentTemplates", "Name" }, null, null, null, null)
          }));
    }
    #endregion

  }
  #region Messages
  public sealed partial class CampaignTemplate : pb::IMessage<CampaignTemplate> {
    private static readonly pb::MessageParser<CampaignTemplate> _parser = new pb::MessageParser<CampaignTemplate>(() => new CampaignTemplate());
    private pb::UnknownFieldSet _unknownFields;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pb::MessageParser<CampaignTemplate> Parser { get { return _parser; } }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pbr::MessageDescriptor Descriptor {
      get { return global::Ares.Core.CampaignTemplateReflection.Descriptor.MessageTypes[0]; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    pbr::MessageDescriptor pb::IMessage.Descriptor {
      get { return Descriptor; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public CampaignTemplate() {
      OnConstruction();
    }

    partial void OnConstruction();

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public CampaignTemplate(CampaignTemplate other) : this() {
      experimentTemplates_ = other.experimentTemplates_.Clone();
      name_ = other.name_;
      _unknownFields = pb::UnknownFieldSet.Clone(other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public CampaignTemplate Clone() {
      return new CampaignTemplate(this);
    }

    /// <summary>Field number for the "ExperimentTemplates" field.</summary>
    public const int ExperimentTemplatesFieldNumber = 1;
    private static readonly pb::FieldCodec<global::Ares.Core.ExperimentTemplate> _repeated_experimentTemplates_codec
        = pb::FieldCodec.ForMessage(10, global::Ares.Core.ExperimentTemplate.Parser);
    private readonly pbc::RepeatedField<global::Ares.Core.ExperimentTemplate> experimentTemplates_ = new pbc::RepeatedField<global::Ares.Core.ExperimentTemplate>();
    /// <summary>
    /// Collection of experiment templates to be used for generating executable experiments
    /// </summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public pbc::RepeatedField<global::Ares.Core.ExperimentTemplate> ExperimentTemplates {
      get { return experimentTemplates_; }
    }

    /// <summary>Field number for the "Name" field.</summary>
    public const int NameFieldNumber = 2;
    private string name_ = "";
    /// <summary>
    /// Name of the campaign. Doesn't have to be descriptive, and is used for lookup
    /// </summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public string Name {
      get { return name_; }
      set {
        name_ = pb::ProtoPreconditions.CheckNotNull(value, "value");
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override bool Equals(object other) {
      return Equals(other as CampaignTemplate);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public bool Equals(CampaignTemplate other) {
      if (ReferenceEquals(other, null)) {
        return false;
      }
      if (ReferenceEquals(other, this)) {
        return true;
      }
      if(!experimentTemplates_.Equals(other.experimentTemplates_)) return false;
      if (Name != other.Name) return false;
      return Equals(_unknownFields, other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override int GetHashCode() {
      int hash = 1;
      hash ^= experimentTemplates_.GetHashCode();
      if (Name.Length != 0) hash ^= Name.GetHashCode();
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
      experimentTemplates_.WriteTo(output, _repeated_experimentTemplates_codec);
      if (Name.Length != 0) {
        output.WriteRawTag(18);
        output.WriteString(Name);
      }
      if (_unknownFields != null) {
        _unknownFields.WriteTo(output);
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public int CalculateSize() {
      int size = 0;
      size += experimentTemplates_.CalculateSize(_repeated_experimentTemplates_codec);
      if (Name.Length != 0) {
        size += 1 + pb::CodedOutputStream.ComputeStringSize(Name);
      }
      if (_unknownFields != null) {
        size += _unknownFields.CalculateSize();
      }
      return size;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void MergeFrom(CampaignTemplate other) {
      if (other == null) {
        return;
      }
      experimentTemplates_.Add(other.experimentTemplates_);
      if (other.Name.Length != 0) {
        Name = other.Name;
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
            experimentTemplates_.AddEntriesFrom(input, _repeated_experimentTemplates_codec);
            break;
          }
          case 18: {
            Name = input.ReadString();
            break;
          }
        }
      }
    }

  }

  #endregion

}

#endregion Designer generated code
