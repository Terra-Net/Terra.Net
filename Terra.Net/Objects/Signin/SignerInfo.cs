namespace Terra.Net.Objects.Signin
{
    public class SignerInfo
    {
    }
    /*export class SignerInfo {
  constructor(
    public public_key: PublicKey,
    public sequence: number,
    public mode_info: ModeInfo
  ) {}

  public static fromData(data: SignerInfo.Data): SignerInfo {
    return new SignerInfo(
      PublicKey.fromData(data.public_key ?? new SimplePublicKey('').toData()),
      Number.parseInt(data.sequence),
      ModeInfo.fromData(data.mode_info)
    );
  }

  public toData(): SignerInfo.Data {
    const { public_key, sequence, mode_info } = this;
    return {
      mode_info: mode_info.toData(),
      public_key: public_key?.toData() || null,
      sequence: sequence.toFixed(),
    };
  }

  public static fromProto(proto: SignerInfo.Proto): SignerInfo {
    return new SignerInfo(
      PublicKey.fromProto(proto.publicKey ?? new SimplePublicKey('').packAny()),
      proto.sequence.toNumber(),
      ModeInfo.fromProto(proto.modeInfo as ModeInfo_pb)
    );
  }

  public toProto(): SignerInfo.Proto {
    const { public_key, sequence, mode_info } = this;
    return SignerInfo_pb.fromPartial({
      modeInfo: mode_info.toProto(),
      publicKey: public_key?.packAny(),
      sequence: Long.fromNumber(sequence),
    });
  }
}*/
}