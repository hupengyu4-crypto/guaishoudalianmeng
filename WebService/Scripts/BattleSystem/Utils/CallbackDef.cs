namespace BattleSystem
{
    public delegate void CallBack();
    public delegate void CallBack<T>(T arg0);
    public delegate void CallBack<T, T1>(T arg0, T1 arg1);
    public delegate void CallBack<T, T1, T2>(T arg0, T1 arg1, T2 args2);
    public delegate void CallBack<T, T1, T2, T3>(T arg0, T1 arg1, T2 args2, T3 args3);
    public delegate void CallBack<T, T1, T2, T3, T4>(T arg0, T1 arg1, T2 args2, T3 args3, T4 args4);
    public delegate void CallBack<T, T1, T2, T3, T4, T5>(T arg0, T1 arg1, T2 args2, T3 args3, T4 args4, T5 args5);



    public delegate bool CallbackBool();
    public delegate bool CallbackBool<T>(T arg0);
    public delegate bool CallbackBool<T, T1>(T arg0, T1 arg1);

    public delegate object CallbackObj();
    public delegate object CallbackObj<T>(T arg0);
    public delegate object CallbackObj<T, T1>(T arg0, T1 arg1);

    public delegate R RCallback<R>();
    public delegate R RCallback<R, T>(T arg);
    public delegate R RCallback<R, T, T1, T2, T3, T4, T5>(T arg0, T1 arg1, T2 args2, T3 args3, T4 args4, T5 args5);

    public delegate TOut CallbackOut<out TOut>();
    public delegate TOut CallbackOut<in T, out TOut>(T arg0);
    public delegate TOut CallbackOut<in T, in T1, out TOut>(T arg0, T1 arg1);
    public delegate TOut CallbackOut<in T, in T1, in T2, out TOut>(T arg0, T1 arg1, T2 arg2);
    public delegate TOut CallbackOut<in T, in T1, in T2, in T3, out TOut>(T arg0, T1 arg1, T2 args2, T3 args3);
}