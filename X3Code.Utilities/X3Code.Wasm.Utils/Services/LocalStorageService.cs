using System.Text.Json;
using Microsoft.JSInterop;
using X3Code.Wasm.Utils.Services.Interfaces;

namespace X3Code.Wasm.Utils.Services;

/// <summary>
/// Provides functions for using the javascript local storage service in your browser
/// <remarks>For best usage, register me in your DI-container</remarks>
/// </summary>
public class LocalStorageService : ILocalStorageService
{
    private readonly IJSRuntime _jsRuntime;
    
    public LocalStorageService(IJSRuntime jsRuntime)
    {
        _jsRuntime = jsRuntime;
    }
    
    /// <summary>
    /// Returns the saved object from local storage by it's key. Returns default if not found. 
    /// </summary>
    /// <param name="key">The key for the needed item</param>
    /// <typeparam name="T">The type for the item</typeparam>
    /// <returns>On success, the parsed item as T</returns>
    public async Task<T?> GetItem<T>(string key)
    {
        var json = await _jsRuntime.InvokeAsync<string>("localStorage.getItem", key);
        if (string.IsNullOrWhiteSpace(json))
            return default;
        
        var result = JsonSerializer.Deserialize<T>(json);
        return result ?? default;
    }

    /// <summary>
    /// Saves any object into local storage
    /// </summary>
    /// <param name="key">The key for the object to store</param>
    /// <param name="value">The object itself, needs to be serializable</param>
    /// <typeparam name="T">Object type</typeparam>
    public async Task SetItem<T>(string key, T value)
    {
        await _jsRuntime.InvokeVoidAsync("localStorage.setItem", key, JsonSerializer.Serialize(value));
    }

    /// <summary>
    /// Removes the object by it's key from the local storage
    /// </summary>
    /// <param name="key">The key for the item to remove</param>
    public async Task RemoveItem(string key)
    {
        await _jsRuntime.InvokeVoidAsync("localStorage.removeItem", key);
    }
}