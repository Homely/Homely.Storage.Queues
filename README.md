# Homely - Storage Queues library.

This library contains some helpers when working with Queue Storage.  
A common pattern when working with queues is to serialize/deserialize complex objects to be stored in a queue message. This library helps simplify this process - both ways. The content of the queue needs to be a string, so any value is converted either into a string representation (when a 'simple' type) -or- a string-JSON representation (when a 'complex' type).

### What's a Simple / Complex Type?
- Simple: a [.NET Primitive](https://docs.microsoft.com/en-us/dotnet/api/system.type.isprimitive?view=netframework-4.7.2#remarks) or `string` or `decimal`
- Complex: everything else. Usually a custom class.

### Azure Storage only?
The library is (currently) only targetting Azure Storage Queues. Why not other queues like RabbitMQ or AWS? Because:

- We (@Homely) don't work with AWS.
- There are [docker images](https://hub.docker.com/r/arafato/azurite/) for Azure Storage (for localhost development) - so we don't need other implementations @Homely.
- We will accept Pull Requeust for other platforms, though :heart:


# Samples

- `AddMessageAsync` : this will accept any object. If it is a simple object (like a `string` or `int`) it will just a insert that value _as is_ into the queue. Otherwise, it will serialize the object to JSON.

e.g. 
```
await queue.AddMessageAsync(1);            // int '1' pushed as a string "1".
await queue.AddMessageAsync("hi there!");  // string 'Hi there' pushed. 
await queue.AddMessageAsync(new Foo());    // serialized to JSON.
await queue.AddMessageAsync(myListOfFoos); // serialized to JSON.
```

- `GetMessageAsync` : this will retrieve the item from the queue and if the destination type is a specific complex object, it will then attempt to deserialize the message into that complex object.

e.g. 
```
// No 'Type' provided. Assumption: message content is not json and will therefore not be deserialized.
var myMessage = await queue.GetMessageAsync();
var content = myMessage.Model; // This is a string.


// 'Type' is provided. Assumption: message content is serialized as JSON.
var myFooMessage = await queue.GetMessageAsync<Foo>();
var foo = myFooMessage.Model; // the value/content of the message, which is a 'Foo'.


// Other points of interest (which help if you wish to delete this message, later)
// myFooMessage.Id == the queue message Id.
// myFooMessage.Receipt == the queue message receipt.

```

---

## Contributing

Discussions and pull requests are encouraged :) Please ask all general questions in this repo or pick a specialized repo for specific, targetted issues. We also have a [contributing](https://github.com/Homely/Homely/blob/master/CONTRIBUTING.md) document which goes into detail about how to do this.

## Code of Conduct
Yep, we also have a [code of conduct](https://github.com/Homely/Homely/blob/master/CODE_OF_CONDUCT.md) which applies to all repositories in the (GitHub) Homely organisation.

## Feedback
Yep, refer to the [contributing page](https://github.com/Homely/Homely/blob/master/CONTRIBUTING.md) about how best to give feedback - either good or needs-improvement :)

---
