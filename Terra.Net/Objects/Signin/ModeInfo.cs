using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Terra.Net.Objects.Signin
{
    internal class ModeInfo
    {
    }
    /*export class ModeInfo {
  public single?: ModeInfo.Single;
  public multi?: ModeInfo.Multi;
  constructor(mode_info: ModeInfo.Single | ModeInfo.Multi) {
    if (mode_info instanceof ModeInfo.Single) {
      this.single = mode_info;
    } else {
      this.multi = mode_info;
    }
  }


  public static fromProto(proto: ModeInfo.Proto): ModeInfo {
    const singleMode = proto.single;
    const multiMode = proto.multi;

    return new ModeInfo(
      singleMode
        ? ModeInfo.Single.fromProto(singleMode)
        : ModeInfo.Multi.fromProto(multiMode as ModeInfoMulti_pb)
    );
  }

  public toProto(): ModeInfo.Proto {
    return ModeInfo_pb.fromPartial({
      multi: this.multi?.toProto(),
      single: this.single?.toProto(),
    });
  }
}*/
}
