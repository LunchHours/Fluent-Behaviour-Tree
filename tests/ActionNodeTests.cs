using FluentBehaviourTree;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace tests
{
    public class ActionNodeTests
    {
        [Fact]
        public void can_run_action()
        {
            var context = new object();

            var invokeCount = 0;
            var testObject = 
                new ActionNode<object>(
                    "some-action", 
                    t =>
                    {
                        Assert.Equal(context, t);

                        ++invokeCount;
                        return BehaviourTreeStatus.Running;
                    }
                );

            Assert.Equal(BehaviourTreeStatus.Running, testObject.Tick(context));
            Assert.Equal(1, invokeCount);            
        }
    }
}
