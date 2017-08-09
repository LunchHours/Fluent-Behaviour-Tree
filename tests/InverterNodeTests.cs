using FluentBehaviourTree;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace tests
{
    public class InverterNodeTests
    {
        InverterNode<object> testObject;

        void Init()
        {
            testObject = new InverterNode<object>("some-node");
        }

        [Fact]
        public void ticking_with_no_child_node_throws_exception()
        {
            Init();

            Assert.Throws<ApplicationException>(
                () => testObject.Tick(new object())
            );
        }

        [Fact]
        public void inverts_success_of_child_node()
        {
            Init();

            var context = new object();

            var mockChildNode = new Mock<IBehaviourTreeNode<object>>();
            mockChildNode
                .Setup(m => m.Tick(context))
                .Returns(BehaviourTreeStatus.Success);

            testObject.AddChild(mockChildNode.Object);

            Assert.Equal(BehaviourTreeStatus.Failure, testObject.Tick(context));

            mockChildNode.Verify(m => m.Tick(context), Times.Once());
        }

        [Fact]
        public void inverts_failure_of_child_node()
        {
            Init();

            var context = new object();

            var mockChildNode = new Mock<IBehaviourTreeNode<object>>();
            mockChildNode
                .Setup(m => m.Tick(context))
                .Returns(BehaviourTreeStatus.Failure);

            testObject.AddChild(mockChildNode.Object);

            Assert.Equal(BehaviourTreeStatus.Success, testObject.Tick(context));

            mockChildNode.Verify(m => m.Tick(context), Times.Once());
        }

        [Fact]
        public void pass_through_running_of_child_node()
        {
            Init();

            var context = new object();

            var mockChildNode = new Mock<IBehaviourTreeNode<object>>();
            mockChildNode
                .Setup(m => m.Tick(context))
                .Returns(BehaviourTreeStatus.Running);

            testObject.AddChild(mockChildNode.Object);

            Assert.Equal(BehaviourTreeStatus.Running, testObject.Tick(context));

            mockChildNode.Verify(m => m.Tick(context), Times.Once());
        }

        [Fact]
        public void adding_more_than_a_single_child_throws_exception()
        {
            Init();

            var mockChildNode1 = new Mock<IBehaviourTreeNode<object>>();
            testObject.AddChild(mockChildNode1.Object);

            var mockChildNode2 = new Mock<IBehaviourTreeNode<object>>();
            Assert.Throws<ApplicationException>(() => 
                testObject.AddChild(mockChildNode2.Object)
            );
        }


    }
}
