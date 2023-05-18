// Cache storage
// https://blazorschool.com/tutorial/blazor-wasm/dotnet6/cache-storage-658620

// https://googlechrome.github.io/samples/service-worker/window-caches/
// window.caches.keys()
// "blazor-resources-/"

async function openCacheStorage() {
    return await window.caches.open("blazor-resources-/");
}

function createRequest(url, method, body = "") {
    let requestInit = { method: method };

    if (body != "") {
        requestInit.body = body;
    }

    let request = new Request(url, requestInit);

    return request;
}

export async function store(url, method, body = "", responseString) {
    let cache = await openCacheStorage();
    let request = createRequest(url, method, body);
    let response = new Response(responseString);
    await cache.put(request, response);
}

export async function get(url, method, body = "") {
    let cache = await openCacheStorage();
    let request = createRequest(url, method, body);
    let response = await cache.match(request);

    if (response == undefined) {
        return "";
    }

    let result = await response.text();

    return result;
}

export async function remove(url, method, body = "") {
    let cache = await openCacheStorage();
    let request = createRequest(url, method, body);
    await cache.delete(request);
}

export async function removeAll() {
    let cache = await openCacheStorage();
    let requests = await cache.keys();

    for (let i = 0; i < requests.length; i++) {
        await cache.delete(requests[i]);
    }
}