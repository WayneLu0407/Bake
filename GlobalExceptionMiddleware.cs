public class GlobalExceptionMiddleware
{
    private readonly RequestDelegate _next;

    public GlobalExceptionMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    // 關鍵在下面這個方法，名稱必須一模一樣！
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            // 呼叫下一個中間件
            await _next(context);
        }
        catch (Exception ex)
        {
            // 這裡處理錯誤邏輯
            await HandleExceptionAsync(context, ex);
        }
    }

    private static Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        // 這裡寫你想回傳給前端的錯誤資訊
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = 500;
        return context.Response.WriteAsync("伺服器發生錯誤，請稍後再試。");
    }
}