using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CQRS.Core.Events;

namespace Post.Common.Events;

public class PostRemovedEvent: BaseEvent
{
    protected PostRemovedEvent(): base(nameof(PostRemovedEvent))
    {

    }

    

}
