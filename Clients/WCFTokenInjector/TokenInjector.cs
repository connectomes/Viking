﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Text;
using System.Threading.Tasks;

namespace Viking.Tokens
{

    public class TokenInjector : System.ServiceModel.Dispatcher.IClientMessageInspector
    {
        public static string BearerTokenAuthority = null;
        public static IdentityModel.Client.TokenResponse BearerToken = null;

        public void AfterReceiveReply(ref Message reply, object correlationState)
        {
            return;
        }

        public object BeforeSendRequest(ref Message request, IClientChannel channel)
        {
            if(BearerTokenAuthority != null && BearerToken != null)
                request.Headers.Add(MessageHeader.CreateHeader("Bearer", BearerTokenAuthority, BearerToken.AccessToken));
            return null;
        }
    }
}
