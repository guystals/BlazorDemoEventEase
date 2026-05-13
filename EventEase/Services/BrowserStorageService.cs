using Microsoft.JSInterop;

namespace EventEase.Services;

public class BrowserStorageService
{
    private readonly IJSRuntime _jsRuntime;
    private readonly ILogger<BrowserStorageService> _logger;

    public BrowserStorageService(IJSRuntime jsRuntime, ILogger<BrowserStorageService> logger)
    {
        _jsRuntime = jsRuntime;
        _logger = logger;
    }

    // LocalStorage methods (persists across browser sessions)
    public async Task<string?> GetFromLocalStorageAsync(string key)
    {
        try
        {
            return await _jsRuntime.InvokeAsync<string?>("localStorage.getItem", key);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to read localStorage key {StorageKey}.", key);
            return null;
        }
    }

    public async Task SetInLocalStorageAsync(string key, string value)
    {
        try
        {
            await _jsRuntime.InvokeVoidAsync("localStorage.setItem", key, value);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to write localStorage key {StorageKey}.", key);
        }
    }

    public async Task RemoveFromLocalStorageAsync(string key)
    {
        try
        {
            await _jsRuntime.InvokeVoidAsync("localStorage.removeItem", key);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to remove localStorage key {StorageKey}.", key);
        }
    }

    public async Task ClearLocalStorageAsync()
    {
        try
        {
            await _jsRuntime.InvokeVoidAsync("localStorage.clear");
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to clear localStorage.");
        }
    }

    // SessionStorage methods (cleared when browser tab is closed)
    public async Task<string?> GetFromSessionStorageAsync(string key)
    {
        try
        {
            return await _jsRuntime.InvokeAsync<string?>("sessionStorage.getItem", key);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to read sessionStorage key {StorageKey}.", key);
            return null;
        }
    }

    public async Task SetInSessionStorageAsync(string key, string value)
    {
        try
        {
            await _jsRuntime.InvokeVoidAsync("sessionStorage.setItem", key, value);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to write sessionStorage key {StorageKey}.", key);
        }
    }

    public async Task RemoveFromSessionStorageAsync(string key)
    {
        try
        {
            await _jsRuntime.InvokeVoidAsync("sessionStorage.removeItem", key);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to remove sessionStorage key {StorageKey}.", key);
        }
    }

    public async Task ClearSessionStorageAsync()
    {
        try
        {
            await _jsRuntime.InvokeVoidAsync("sessionStorage.clear");
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to clear sessionStorage.");
        }
    }
}
