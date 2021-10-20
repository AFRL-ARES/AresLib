// <auto-generated>
//     Generated by the protocol buffer compiler.  DO NOT EDIT!
//     source: CommandMetadata.proto
// </auto-generated>
#pragma warning disable 1591, 0612, 3021
#region Designer generated code

using pb = global::Google.Protobuf;
using pbc = global::Google.Protobuf.Collections;
using pbr = global::Google.Protobuf.Reflection;
using scg = global::System.Collections.Generic;
namespace Ares.Core {

  /// <summary>Holder for reflection information generated from CommandMetadata.proto</summary>
  public static partial class CommandMetadataReflection {

    #region Descriptor
    /// <summary>File descriptor for CommandMetadata.proto</summary>
    public static pbr::FileDescriptor Descriptor {
      get { return descriptor; }
    }
    private static pbr::FileDescriptor descriptor;

    static CommandMetadataReflection() {
      byte[] descriptorData = global::System.Convert.FromBase64String(
          string.Concat(
            "ChVDb21tYW5kTWV0YWRhdGEucHJvdG8SCWFyZXMuY29yZRoeQ29tbWFuZFBh",
            "cmFtZXRlck1ldGFkYXRhLnByb3RvIn8KD0NvbW1hbmRNZXRhZGF0YRIMCgRO",
            "YW1lGAEgASgJEhMKC0Rlc2NyaXB0aW9uGAIgASgJEhIKCkRldmljZU5hbWUY",
            "AyABKAkSNQoITWV0YWRhdGEYBCADKAsyIy5hcmVzLmNvcmUuQ29tbWFuZFBh",
            "cmFtZXRlck1ldGFkYXRhYgZwcm90bzM="));
      descriptor = pbr::FileDescriptor.FromGeneratedCode(descriptorData,
          new pbr::FileDescriptor[] { global::Ares.Core.CommandParameterMetadataReflection.Descriptor, },
          new pbr::GeneratedClrTypeInfo(null, null, new pbr::GeneratedClrTypeInfo[] {
            new pbr::GeneratedClrTypeInfo(typeof(global::Ares.Core.CommandMetadata), global::Ares.Core.CommandMetadata.Parser, new[]{ "Name", "Description", "DeviceName", "Metadata" }, null, null, null, null)
          }));
    }
    #endregion

  }
  #region Messages
  public sealed partial class CommandMetadata : pb::IMessage<CommandMetadata> {
    private static readonly pb::MessageParser<CommandMetadata> _parser = new pb::MessageParser<CommandMetadata>(() => new CommandMetadata());
    private pb::UnknownFieldSet _unknownFields;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pb::MessageParser<CommandMetadata> Parser { get { return _parser; } }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pbr::MessageDescriptor Descriptor {
      get { return global::Ares.Core.CommandMetadataReflection.Descriptor.MessageTypes[0]; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    pbr::MessageDescriptor pb::IMessage.Descriptor {
      get { return Descriptor; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public CommandMetadata() {
      OnConstruction();
    }

    partial void OnConstruction();

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public CommandMetadata(CommandMetadata other) : this() {
      name_ = other.name_;
      description_ = other.description_;
      deviceName_ = other.deviceName_;
      metadata_ = other.metadata_.Clone();
      _unknownFields = pb::UnknownFieldSet.Clone(other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public CommandMetadata Clone() {
      return new CommandMetadata(this);
    }

    /// <summary>Field number for the "Name" field.</summary>
    public const int NameFieldNumber = 1;
    private string name_ = "";
    /// <summary>
    /// Name of the command, registered by a device
    /// </summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public string Name {
      get { return name_; }
      set {
        name_ = pb::ProtoPreconditions.CheckNotNull(value, "value");
      }
    }

    /// <summary>Field number for the "Description" field.</summary>
    public const int DescriptionFieldNumber = 2;
    private string description_ = "";
    /// <summary>
    /// Helpful text describing the command
    /// </summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public string Description {
      get { return description_; }
      set {
        description_ = pb::ProtoPreconditions.CheckNotNull(value, "value");
      }
    }

    /// <summary>Field number for the "DeviceName" field.</summary>
    public const int DeviceNameFieldNumber = 3;
    private string deviceName_ = "";
    /// <summary>
    /// Name of the device to lookup for execution/generation of the command
    /// </summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public string DeviceName {
      get { return deviceName_; }
      set {
        deviceName_ = pb::ProtoPreconditions.CheckNotNull(value, "value");
      }
    }

    /// <summary>Field number for the "Metadata" field.</summary>
    public const int MetadataFieldNumber = 4;
    private static readonly pb::FieldCodec<global::Ares.Core.CommandParameterMetadata> _repeated_metadata_codec
        = pb::FieldCodec.ForMessage(34, global::Ares.Core.CommandParameterMetadata.Parser);
    private readonly pbc::RepeatedField<global::Ares.Core.CommandParameterMetadata> metadata_ = new pbc::RepeatedField<global::Ares.Core.CommandParameterMetadata>();
    /// <summary>
    /// Collection of metadata describing parameters/arguments
    /// </summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public pbc::RepeatedField<global::Ares.Core.CommandParameterMetadata> Metadata {
      get { return metadata_; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override bool Equals(object other) {
      return Equals(other as CommandMetadata);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public bool Equals(CommandMetadata other) {
      if (ReferenceEquals(other, null)) {
        return false;
      }
      if (ReferenceEquals(other, this)) {
        return true;
      }
      if (Name != other.Name) return false;
      if (Description != other.Description) return false;
      if (DeviceName != other.DeviceName) return false;
      if(!metadata_.Equals(other.metadata_)) return false;
      return Equals(_unknownFields, other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override int GetHashCode() {
      int hash = 1;
      if (Name.Length != 0) hash ^= Name.GetHashCode();
      if (Description.Length != 0) hash ^= Description.GetHashCode();
      if (DeviceName.Length != 0) hash ^= DeviceName.GetHashCode();
      hash ^= metadata_.GetHashCode();
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
      if (Description.Length != 0) {
        output.WriteRawTag(18);
        output.WriteString(Description);
      }
      if (DeviceName.Length != 0) {
        output.WriteRawTag(26);
        output.WriteString(DeviceName);
      }
      metadata_.WriteTo(output, _repeated_metadata_codec);
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
      if (Description.Length != 0) {
        size += 1 + pb::CodedOutputStream.ComputeStringSize(Description);
      }
      if (DeviceName.Length != 0) {
        size += 1 + pb::CodedOutputStream.ComputeStringSize(DeviceName);
      }
      size += metadata_.CalculateSize(_repeated_metadata_codec);
      if (_unknownFields != null) {
        size += _unknownFields.CalculateSize();
      }
      return size;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void MergeFrom(CommandMetadata other) {
      if (other == null) {
        return;
      }
      if (other.Name.Length != 0) {
        Name = other.Name;
      }
      if (other.Description.Length != 0) {
        Description = other.Description;
      }
      if (other.DeviceName.Length != 0) {
        DeviceName = other.DeviceName;
      }
      metadata_.Add(other.metadata_);
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
            Description = input.ReadString();
            break;
          }
          case 26: {
            DeviceName = input.ReadString();
            break;
          }
          case 34: {
            metadata_.AddEntriesFrom(input, _repeated_metadata_codec);
            break;
          }
        }
      }
    }

  }

  #endregion

}

#endregion Designer generated code
