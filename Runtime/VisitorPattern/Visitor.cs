﻿using UnityEngine;

namespace Mimizh.UnityUtilities.VisitorPattern
{
    public interface IVisitor {
        void Visit<T>(T visitable) where T : Component, IVisitable;
    }

    public interface IVisitable {
        void Accept(IVisitor visitor);
    }
}