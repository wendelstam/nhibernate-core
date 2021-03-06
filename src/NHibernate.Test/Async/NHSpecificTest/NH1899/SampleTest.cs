﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by AsyncGenerator.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------


using System.Collections.Generic;

using NHibernate.Dialect;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH1899
{
    using System.Threading.Tasks;
    [TestFixture]
    public class SampleTestAsync : BugTestCase
    {
        protected override void OnSetUp()
        {
            base.OnSetUp();
            using (ISession session = OpenSession())
            {
                Parent entity = new Parent();
                entity.Id = 1;
                entity.Relations = new Dictionary<Key, Value>();
                entity.Relations.Add(Key.One, Value.ValOne);
                entity.Relations.Add(Key.Two, Value.ValTwo);
                session.Save(entity);
                session.Flush();
            }
        }

        protected override void OnTearDown()
        {
            base.OnTearDown();
            using (ISession session = OpenSession())
            {
                string hql = "from System.Object";
                session.Delete(hql);
                session.Flush();
            }
        }

        [Test]
        public async Task ShouldNotThrowOnMergeAsync()
        {
            Parent entity;

            using (ISession session = OpenSession())
            {
                entity = await (session.GetAsync<Parent>(1));
                session.Close();
                session.Dispose();
            }

            using (ISession session2 = OpenSession()) 
			{
                entity = await (session2.MergeAsync(entity));
            }
        }
    }
}
