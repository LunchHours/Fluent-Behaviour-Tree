using FluentBehaviourTree;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace tests
{
    public class SelectorNodeTests
    {
        SelectorNode testObject;

        void Init()
        {
            testObject = new SelectorNode("some-selector");
        }

        [Fact]
        public void runs_the_first_node_if_it_succeeds()
        {
            Init();

            var context = new object();

            var mockChild1 = new Mock<IBehaviourTreeNode>();
            mockChild1
                .Setup(m => m.Tick(context))
                .Returns(BehaviourTreeStatus.Success);

            var mockChild2 = new Mock<IBehaviourTreeNode>();

            testObject.AddChild(mockChild1.Object);
            testObject.AddChild(mockChild2.Object);

            Assert.Equal(BehaviourTreeStatus.Success, testObject.Tick(context));

            mockChild1.Verify(m => m.Tick(context), Times.Once());
            mockChild2.Verify(m => m.Tick(context), Times.Never());
        }

        [Fact]
        public void stops_on_the_first_node_when_it_is_running()
        {
            Init();

            var context = new object();

            var mockChild1 = new Mock<IBehaviourTreeNode>();
            mockChild1
                .Setup(m => m.Tick(context))
                .Returns(BehaviourTreeStatus.Running);

            var mockChild2 = new Mock<IBehaviourTreeNode>();

            testObject.AddChild(mockChild1.Object);
            testObject.AddChild(mockChild2.Object);

            Assert.Equal(BehaviourTreeStatus.Running, testObject.Tick(context));

            mockChild1.Verify(m => m.Tick(context), Times.Once());
            mockChild2.Verify(m => m.Tick(context), Times.Never());
        }

        [Fact]
        public void runs_the_second_node_if_the_first_fails()
        {
            Init();

            var context = new object();

            var mockChild1 = new Mock<IBehaviourTreeNode>();
            mockChild1
                .Setup(m => m.Tick(context))
                .Returns(BehaviourTreeStatus.Failure);

            var mockChild2 = new Mock<IBehaviourTreeNode>();
            mockChild2
                .Setup(m => m.Tick(context))
                .Returns(BehaviourTreeStatus.Success);

            testObject.AddChild(mockChild1.Object);
            testObject.AddChild(mockChild2.Object);

            Assert.Equal(BehaviourTreeStatus.Success, testObject.Tick(context));

            mockChild1.Verify(m => m.Tick(context), Times.Once());
            mockChild2.Verify(m => m.Tick(context), Times.Once());
        }

        [Fact]
        public void fails_when_all_children_fail()
        {
            Init();

            var context = new object();

            var mockChild1 = new Mock<IBehaviourTreeNode>();
            mockChild1
                .Setup(m => m.Tick(context))
                .Returns(BehaviourTreeStatus.Failure);

            var mockChild2 = new Mock<IBehaviourTreeNode>();
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

