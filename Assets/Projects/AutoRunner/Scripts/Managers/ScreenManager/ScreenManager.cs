using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenManager : MonoBehaviour
{
    public static ScreenManager Instance { get; private set; }

    Stack<IScreen> _screenStack;

    private void Awake()
    {
        Instance = this;

        _screenStack = new Stack<IScreen>();
    }

    public void Push(IScreen newScreen)
    {        
        if (_screenStack.Count > 0)
            _screenStack.Peek().Deactivate();

        _screenStack.Push(newScreen);

        newScreen.Activate();
    }

    public void Pop()
    {
        if (_screenStack.Count <= 1) return;

        _screenStack.Pop().Free();

        _screenStack.Peek().Activate();
    }
}
