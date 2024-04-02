using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using Aplib.Core;
using Aplib.Core.Belief;
using Aplib.Core.Desire;
using Aplib.Core.Desire.Goals;
using Aplib.Core.Intent.Actions;
using Aplib.Core.Intent.Tactics;
using Aplib.Integrations.Unity;

public class MyFirstBeliefSet : BeliefSet
{
}

public class MyFirstAplibTest
{
    // A UnityTest behaves like a coroutine in Play Mode. In Edit Mode you can use
    // `yield return null;` to skip a frame.
    [UnityTest]
    public IEnumerator MySecondAplibTestWithEnumeratorPasses()
    {
        Action doNothing = new Action(() => { });
        Tactic nothingTactic = new PrimitiveTactic(doNothing);
        Goal nothingGoal = new Goal(tactic: nothingTactic, predicate: () => true);
        
        MyFirstBeliefSet beliefSet = new MyFirstBeliefSet();
        DesireSet<MyFirstBeliefSet> desireSet = new DesireSet<MyFirstBeliefSet>(new PrimitiveGoalStructure<MyFirstBeliefSet>(nothingGoal));
        
        BdiAgent<MyFirstBeliefSet> agent = new BdiAgent<MyFirstBeliefSet>(beliefSet, desireSet);

        AplibRunner tr = new(agent);
        
        yield return tr.Test();
        
        Assert.True(true);
    }
}
