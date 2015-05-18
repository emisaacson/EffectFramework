EffectFramework v0.0.1
====================
EffectFramework is a very simple data framework for storing records to a database using a flexible EAV schema.
It's primarily a tool for an open-schema application but it maintains some important relational features. It's
main feature is effective dating fields are first class citizens in this model so it's particularly useful
for applications that require a flexible field structure as well as being able to easily reconstruct a record
in its entirety at for any arbitrary date.

This is a DNX project and currently builds on DNX451 and should eventually build on CoreCLR.

Currently an Entity Framework 7 adapter is included but you can use another one via dependency injection.

Features
--------
* Schema defined in data (code or DB) so no database migrations ever needed
* All updates to a record occur within a transaction so there is never a bad state persisted
* Each field update gets a GUID which is checked before persisting a record update
so concurrent updates will never leave a record in a bad state.
* All field collections are effective dated and this is intuitively exposed through the access API
* Binding database models to "Form" objects that can be used (for instance) as ViewModels in an MVC project
* Storage medium agnostic although only support for SQL server is included OOTB
* Full test suite
* (TODO) Hooks for field level access permissions
* (TODO) Audit log of all field updates
* (TODO) Hooks for validation
* (TODO) API for accessing items, entities and fields only defined in the database
* (TODO) Define and store enumerations for fields with restricted values

Not Features
------------
This framework does not help you out with any of these tasks (but are achievable in a number of ways):

* Keeping the entity types consistent between the database and the code

Misc. ToDos
-----------
* Document strings once the APIs are a little more solidified
* Remove Ninject and use MS DI Container

Why?
----
I wrote this for use in another project and thought it might be nice to extract this framework instead
of mixing it up with the application code.

Glossary
--------
* Item: a thing you want to track (e.g., a User or Product)
* Field: a value you want to attach to the Item (currently can be: string, decimal, date, binary)
* Entity: a collection of Fields with an effective start and end date
* Record: a collection of all entities which are effective on some particular date

For example:

A User might have a billing address and a shipping address. His shipping address is New York from
2012-01-01 to 2013-01-01 and Boston from 2013-01-01 to 2014-01-01. In this case, the cities New York
and Boston are Fields, the shipping address is an Entity, collectively his Shipping and Billing address
on 2013-06-01 is a Record, and the User is the Item.

How To Use
----------
First, create an item type:

```c#
    using System;
    using EffectFramework.Core.Models;

    namespace MyApp
    {
        public class MyItemType : ItemType
        {
            protected MyItemType(string Name, int Value, Type Type) : base(Name, Value, Type) { }

            public static readonly MyItemType User = new MyItemType("User", 1, typeof(User));

        }
    }
```

For this as well as the Entity and Field Types, make sure that the same type is registered in the database
with the same ID:

<table>
    <tr><th>ItemTypeID</th><th>Name</th></tr>
    <tr><td>1</td>         <td>User</td></tr>
</table>

Now you can create your item class:

```c#
    using EffectFramework.Core;
    using EffectFramework.Core.Models;
    using EffectFramework.Core.Services;
    using Ninject;
    using Ninject.Parameters;

    namespace MyApp
    {
        public class User : Item
        {
            public override ItemType Type
            {
                get
                {
                    return MyItemType.User;
                }
            }

            public User(IPersistenceService PersistenceService) : base(PersistenceService) { }

            public User(int UserID, IPersistenceService PersistenceService, bool LoadItem = true) : base(UserID, PersistenceService, LoadItem) { }

            public static User GetUserById(int UserID)
            {
                using (IKernel Kernel = new StandardKernel(new Configure()))
                {
                    User User = Kernel.Get<User>(new ConstructorArgument("UserID", UserID));
                    return User;
                }
            }

            public static User CreateUser()
            {
                using (IKernel Kernel = new StandardKernel(new Configure()))
                {
                    User User = Kernel.Get<User>();
                    return User;
                }
            }
        }
    }
```

Then, create your entity types:

```c#
    using System;
    using EffectFramework.Core.Models.Entities;

    namespace MyApp
    {
        public class MyEntityType : EntityType
        {
            public static readonly MyEntityType BillingAddress = new MyEntityType("Billing Address", 1, typeof(BillingAddress));
            public static readonly MyEntityType ShippingAddress = new MyEntityType("Shipping Address", 1, typeof(ShippingAddress));

            protected MyEntityType(string Name, int Value, Type Type) : base(Name, Value, Type) { }
        }
    }
```

And your field types:

```c#
    using EffectFramework.Core.Models.Fields;

    namespace MyApp
    {
        public class MyFieldType : FieldType
        {
            protected MyFieldType(string Name, int Value, DataType DataType) : base(Name, Value, DataType) { }

            public static readonly MyFieldType Address1 = new MyFieldType("Address 1", 1, DataType.Text);
            public static readonly MyFieldType Address2 = new MyFieldType("Address 2", 2, DataType.Text);
            public static readonly MyFieldType City = new MyFieldType("City", 3, DataType.Text);
            public static readonly MyFieldType State = new MyFieldType("State", 4, DataType.Text);
            public static readonly MyFieldType Zip = new MyFieldType("Zip", 5, DataType.Text);
        }
    }
```

And your entities, along with any fields you want included in code:

```c#
    using EffectFramework.Core.Models.Annotations;
    using EffectFramework.Core.Models.Entities;
    using EffectFramework.Core.Models.Fields;
    using EffectFramework.Core.Services;

    namespace MyApp
    {
        public class BillingAddress : EntityBase
        {
            public override EntityType Type
            {
                get
                {
                    return MyEntityType.BillingAddress;
                }
            }

            public BillingAddress() : base() { }

            public BillingAddress(IPersistenceService PersistenceService) : base(PersistenceService) { }

            protected override void WireUpFields()
            {
                Address1 = new FieldString(MyFieldType.Address1, PersistenceService);
                Address2 = new FieldString(MyFieldType.Address2, PersistenceService);
                City = new FieldString(MyFieldType.City, PersistenceService);
                State = new FieldString(MyFieldType.State, PersistenceService);
                Zip = new FieldString(MyFieldType.Zip, PersistenceService);
            }

            public FieldString Address1 { get; private set; }
            public FieldString Address2 { get; private set; }
            public FieldString City { get; private set; }
            public FieldString State { get; private set; }
            public FieldString Zip { get; private set; }
        }
    }
```

You only need to define a field type once then you can use it in as many entities as you'd like. For instance, this could be
an addition Shipping Address entity:

```c#
    using EffectFramework.Core.Models.Annotations;
    using EffectFramework.Core.Models.Entities;
    using EffectFramework.Core.Models.Fields;
    using EffectFramework.Core.Services;

    namespace MyApp
    {
        public class ShippingAddress : EntityBase
        {
            public override EntityType Type
            {
                get
                {
                    return MyEntityType.ShippingAddress;
                }
            }

            public ShippingAddress() : base() { }

            public ShippingAddress(IPersistenceService PersistenceService) : base(PersistenceService) { }

            protected override void WireUpFields()
            {
                Address1 = new FieldString(MyFieldType.Address1, PersistenceService);
                Address2 = new FieldString(MyFieldType.Address2, PersistenceService);
                City = new FieldString(MyFieldType.City, PersistenceService);
                State = new FieldString(MyFieldType.State, PersistenceService);
                Zip = new FieldString(MyFieldType.Zip, PersistenceService);
            }

            public FieldString Address1 { get; private set; }
            public FieldString Address2 { get; private set; }
            public FieldString City { get; private set; }
            public FieldString State { get; private set; }
            public FieldString Zip { get; private set; }
        }
    }
```

Now register your types and you're ready to go:

```c#
    Configure.RegisterTypeClasses<MyItemType, MyEntityType, MyFieldType>();
```

Use the object model to build and persist your item:

```c#
    User MyUser = User.CreateUser();
    
    // Change the current effective date to Jan 1, 2015.
    // User.EffectiveRecord now will be an EntityCollection
    // containing all entities active on the requested
    // date.
    User.EffectiveDate = new DateTime(2015, 1, 1);

    // Create a new entity using the current effective date (and optionally an
    // ending effective date as a parameter).
    ShippingAddress NewAddress = User.EffectiveRecord.GetOrCreateEntity<ShippingAddress>(new DateTime(2016, 1, 1));
    NewAddress.Address1.Value = "123 Main St.";
    NewAddress.Address2.Value = "Apt 4";
    NewAddress.City.Value = "Yorktown";
    NewAddress.State.Value = "NY";
    NewAddress.Zip.Value = "55555";

    // Lets also add a Billing Address, but for a different date range. This one
    // has no end date.
    User.EffectiveDate = new DateTime(2015, 6, 1);
    BillingAddress OtherAddress = User.EffectiveRecord.GetOrCreateEntity<BillingAddress>();
    OtherAddress.Address1.Value = "234 Elm St.";
    OtherAddress.Address2.Value = "Apt 5";
    OtherAddress.City.Value = "Bleeker";
    OtherAddress.State.Value = "MN";
    OtherAddress.Zip.Value = "77777";

    // Any updates are performed in a transaction and checked against
    // hashes for each field in the DB. Optionally pass in a context
    // to manage your own transaction (if you need to update multiple
    // items in a single transaction, for instance).
    User.PersistToDatabase();

    // Reload the user from the database, (not necessary but lets double check we're getting back
    // what is expected).
    User.Load();

    User.EffectiveDate = new DateTime(2015, 1, 1);
    // Should all pass.
    Assert.Equal(2, User.AllEntities.Count());
    Assert.Equal(1, User.EffectiveRecord.AllEntities.Count());
    Assert.Equal(typeof(ShippingAddress), User.EffectiveRecord.AllEntities.First());
    
    // This returns both since both are active on this date
    User.EffectiveDate = new DateTime(2015, 7, 1);
    Assert.Equal(2, User.EffectiveRecord.AllEntities.Count());

    // This is after the end date of the shipping address so
    // only the billing address is still in effect.
    User.EffectiveDate = new DateTime(2016, 2, 1);
    Assert.Equal(1, User.EffectiveRecord.AllEntities.Count());
    Assert.Equal(typeof(BillingAddress), User.EffectiveRecord.AllEntities.First());
```

The rest of the APIs for retreiving, manipulating, and storing entities, items, and fields should be self-explanitory.

Note that currently there is currently no concept of storing a null field value - any nulled field values result in a field
deletion at the database level. Deletion is by flag in all cases and no database relation is ever actually deleted.
