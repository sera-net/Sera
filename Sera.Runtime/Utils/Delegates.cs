using Sera.Core;

namespace Sera.Runtime.Utils.Delegates;

public delegate R FnSerAcceptItem<out R, in V, T>(V visitor, ref T value, int index) where V : ATupleSeraVisitor<R>;

public delegate R FnSerAcceptField<out R, in V, T>(V visitor, ref T value, int field) where V : AStructSeraVisitor<R>;

public delegate R FnSerAcceptUnion<out R, in V, T>(V visitor, ref T value) where V : AUnionSeraVisitor<R>;
