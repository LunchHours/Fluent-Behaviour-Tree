using FluentBehaviourTree;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace tests
{
    public class SequenceNodeTests
    {
        SequenceNode<object> testObject;

        void Init()
        {
            testObject = new SequenceNode<object>("some-sequence");
        }
        
        [Fact]
        public void can_run_all_children_in_order()
        {
            Init();

            var context = new object();

            var callOrder = 0;

            var mockChild1 = new Mock<IBehaviourTreeNode<object>>();
            mockChild1
                .Setup(m => m.Tick(context))
                .Returns(BehaviourTreeStatus.Success)
                .Callback(() =>
                 {
                     Assert.Equal(1, ++callOrder);
                 });

            var mockChild2 = new Mock<IBehaviourTreeNode<object>>();
            mockChild2
                .Setup(m => m.Tick(context))
                .Returns(BehaviourTreeStatus.Success)
                .Callback(() =>
                {
                    Assert.Equal(2, ++callOrder);
                });

            testObject.AddChild(mockChild1.Object);
            testObject.AddChild(mockChild2.Object);

            Assert.Equal(BehaviourTreeStatus.Success, testObject.Tick(context));

            Assert.Equal(2, callOrder);

            mockChild1.Verify(m => m.Tick(context), Times.Once());
            mockChild2.Verify(m => m.Tick(context), Times.Once());
        }

        [Fact]
        public void when_first_child_is_running_second_child_is_supressed()
        {
            Init();

            var context = new object();

            var mockChild1 = new Mock<IBehaviourTreeNode<object>>();
            mockChild1
                .Setup(m => m.Tick(context))
                .Returns(BehaviourTreeStatus.Running);

            var mockChild2 = new Mock<IBehaviourTreeNode<object>>();

            testObject.AddChild(mockChild1.Object);
            testObject.AddChild(mockChild2.Object);

            Assert.Equal(BehaviourTreeStatus.Running, testObject.Tick(context));

            mockChild1.Verify(m => m.Tick(context), Times.Once());
            mockChild2.Verify(m => m.Tick(context), Times.Never());
        }

        [Fact]
        public void when_first_child_fails_then_entire_sequence_fails()
        {
            Init();

            var context = new object();

            var mockChild1 = new Mock<IBehaviourTreeNode<object>>();
            mockChild1
                .Setup(m => m.Tick(context))
                .Returns(BehaviourTreeStatus.Failure);

            var mockChild2 = new Mock<IBehaviourTreeNode<object>>();

            testObject.AddChild(mockChild1.Object);
            testObject.AddChild(mockChild2.Object);

            Assert.Equal(BehaviourTreeStatus.Failure, testObject.Tick(context));

            mockChild1.Verify(m => m.Tick(context), Times.Once());
            mockChild2.Verify(m => m.Tick(context), Times.Never());
        }

        [Fact]
        public void when_second_child_fails_then_entire_sequence_fails()
        {
            Init();

            var context = new object();

            var mockChild1 = new Mock<IBehaviourTreeNode<object>>();
            mockChild1
                .Setup(m => m.Tick(context))
                .Returns(BehaviourTreeStatus.Success);

            var mockChild2 = new Mock<IBehaviourTreeNode<object>>();
            mockChild2
                .Setup(m => m.Tick(context))
                .Returns(BehaviourTreeStatus.Failure);

            testObject.AddChild(mockChild1.Object);
            testObject.AddChild(mockChild2.Object);

            Assert.Equal(BehaviourTreeStatus.Failure, testObject.Tick(context));

            mockChild1.Verify(m => m.Tick(context), Times.Once());
            mockChild2.Verify(m => m.Tick(context), Times.Once());
        }
    }
}
