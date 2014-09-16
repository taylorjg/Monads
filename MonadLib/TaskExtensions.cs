using System;
using System.Threading.Tasks;

namespace MonadLib
{
    public static class TaskMonadExtensions
    {
        public static Task<TA> Unit<TA>(TA a)
        {
            return Task.FromResult(a);
        }

        public static Task<TB> Bind<TA, TB>(this Task<TA> ma, Func<TA, Task<TB>> f)
        {
            return ma.Then(f);
        }

        // http://blogs.msdn.com/b/pfxteam/archive/2010/11/21/10094564.aspx?Redirected=true
        public static Task<T2> Then<T1, T2>(this Task<T1> first, Func<T1, Task<T2>> next)
        {
            if (first == null) throw new ArgumentNullException("first");
            if (next == null) throw new ArgumentNullException("next");

            var tcs = new TaskCompletionSource<T2>();
            first.ContinueWith(_ =>
                {
                    if (first.IsFaulted)
                    {
                        // ReSharper disable PossibleNullReferenceException
                        tcs.TrySetException(first.Exception.InnerExceptions);
                        // ReSharper restore PossibleNullReferenceException
                    }
                    else if (first.IsCanceled) tcs.TrySetCanceled();
                    else
                    {
                        try
                        {
                            var t = next(first.Result);
                            if (t == null) tcs.TrySetCanceled();
                            else
                                t.ContinueWith(__ =>
                                    {
                                        if (t.IsFaulted)
                                        {
                                            // ReSharper disable PossibleNullReferenceException
                                            tcs.TrySetException(t.Exception.InnerExceptions);
                                            // ReSharper restore PossibleNullReferenceException
                                        }
                                        else if (t.IsCanceled) tcs.TrySetCanceled();
                                        else tcs.TrySetResult(t.Result);
                                    }, TaskContinuationOptions.ExecuteSynchronously);
                        }
                        catch (Exception exc)
                        {
                            tcs.TrySetException(exc);
                        }
                    }
                }, TaskContinuationOptions.ExecuteSynchronously);

            return tcs.Task;
        }
    }
}
