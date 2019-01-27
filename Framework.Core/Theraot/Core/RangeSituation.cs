#if FAT
using System;

namespace Theraot.Core
{
    [Flags]
    public enum RangeSituation
    {
        /*
         ???
         */
        Invalid = 0,
        /*
         x =  |---|
         y =         |---|
         */
        BeforeSeparated = 1,
        /*
         x =  |---|
         y =      |---|
         */
        BeforeTouching = 2,
        /*
         x =  |---|
         y =    |---|
         */
        BeforeOverlapped = 4,
        /*
         x =    |---|
         y =  |-------|
         */
        Contained = 8,
        /*
         x =  |---|
         y =  |---|
         */
        Equals = 16,
        /*
         x =  |-------|
         y =    |---|
         */
        Contains = 32,
        /*
         x =      |---|
         y =    |---|
         */
        AfterOverlapped = 64,
        /*
        x =      |---|
        y =  |---|
        */
        AfterTouching = 128,
        /*
        x =         |---|
        y =  |---|
        */
        AfterSeparated = 256,
        /*Before = BeforeSeparated | BeforeOverlapped | BeforeTouching,
        After = AfterSeparated | AfterOverlapped | AfterTouching,
        BeforeNotSeparated = BeforeOverlapped | BeforeTouching,
        AfterNotSeparated = AfterOverlapped | AfterTouching,
        Eclipses = Equals | Contains,
        Eclipsed = Equals | Contained,
        NoTouching = BeforeSeparated | AfterSeparated,
        Touching = BeforeTouching | AfterTouching,
        NoOverlap = BeforeSeparated | BeforeTouching | AfterSeparated | AfterTouching,*/
    }
}

#endif