﻿using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Terra.Net.Objects.Signin
{
    [ProtoContract]
    public class SignDoc
    {
        public SignDoc(string chainId, ulong accountNumber, ulong sequence, AuthInfo)
        {

        }
    }


    /*
     export class SignDoc extends JSONSerializable<
    SignDoc.Amino,
    SignDoc.Data,
    SignDoc.Proto
  > {
    /**
     *
     * @param chain_id ID of blockchain to submit transaction to
     * @param account_number account number on blockchain
     * @param sequence Sequence number (nonce), number of signed previous transactions by
     *    account included on the blockchain at time of broadcast.
     * @param fee transaction fee
     * @param msgs list of messages to include
     * @param memo optional note
     * @param timeout_height optional transaction timeout height, does not support amino
     * @param public_key Signer's public key, only used at direct sign mode
     
    constructor(
    public chain_id: string,
    public account_number: number,
    public sequence: number,
    public auth_info: AuthInfo,
    public tx_body: TxBody
  ) {
    super();
}

public toAmino(): SignDoc.Amino {
    const {
        chain_id,
      account_number,
      sequence,
      tx_body: { memo, messages, timeout_height },
      auth_info: { fee },
    } = this;

    return {
        chain_id,
      account_number: account_number.toString(),
      sequence: sequence.toString(),
      timeout_height:
        timeout_height && timeout_height !== 0
          ? timeout_height.toString()
          : undefined,
      fee: fee.toAmino(),
      msgs: messages.map(m => m.toAmino()),
      memo: memo ?? '',
    };
}

public toData(): SignDoc.Data {
    const { account_number, chain_id, tx_body, auth_info } = this;
    return {
    body_bytes: Buffer.from(tx_body.toBytes()).toString('base64'),
      auth_info_bytes: Buffer.from(auth_info.toBytes()).toString('base64'),
      account_number: account_number.toFixed(),
      chain_id,
    };
}

public toProto(): SignDoc.Proto {
    const { account_number, chain_id, tx_body, auth_info } = this;
    return SignDoc_pb.fromPartial({
    bodyBytes: tx_body.toBytes(),
      authInfoBytes: auth_info.toBytes(),
      accountNumber: Long.fromNumber(account_number),
      chainId: chain_id,
    });
}

public toUnSignedTx(): Tx
{
    return new Tx(this.tx_body, this.auth_info, []);
}

public toBytes(): Uint8Array
{
    return SignDoc_pb.encode(this.toProto()).finish();
}
}

export namespace SignDoc
{
    export interface Amino
    {
        chain_id: string;
    account_number: string;
    sequence: string;
    timeout_height?: string;
    fee: Fee.Amino;
    msgs: Msg.Amino[];
    memo: string;
  }

    export interface Data
    {
        body_bytes: string;
    auth_info_bytes: string;
    chain_id: string;
    account_number: string;
  }

    export type Proto = SignDoc_pb;
}
*/
}