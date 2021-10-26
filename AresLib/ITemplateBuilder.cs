using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ares.Core;
using Google.Protobuf;

namespace AresLib
{
  internal interface ITemplateBuilder<TemplateMessage> : IBuilder<TemplateMessage> where TemplateMessage : IMessage
  {
    TemplateMessage Build();
  }
}
