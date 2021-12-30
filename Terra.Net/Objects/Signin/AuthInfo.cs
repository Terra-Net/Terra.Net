using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Terra.Net.Objects.Signin
{
    /*https://github.com/cosmos/cosmos-sdk/blob/v0.40.0-rc6/docs/architecture/adr-020-protobuf-transaction-encoding.md*/
    [ProtoContract]
    public  class AuthInfo
    {
        public AuthInfo(SignerInfo[] signerInfos, Fee fee)
        {

        }
    }
    /*
     export class AuthInfo {
  constructor(public signer_infos: SignerInfo[], public fee: Fee) {}

 
  public static fromProto(proto: AuthInfo.Proto): AuthInfo {
    return new AuthInfo(
      proto.signerInfos.map(s => SignerInfo.fromProto(s)),
      Fee.fromProto(proto.fee as Fee.Proto)
    );
  }

  public toProto(): AuthInfo.Proto {
    return AuthInfo_pb.fromPartial({
      fee: this.fee.toProto(),
      signerInfos: this.signer_infos.map(info => info.toProto()),
    });
  }

  public toBytes(): Uint8Array {
    return AuthInfo_pb.encode(this.toProto()).finish();
  }
}*/
}
