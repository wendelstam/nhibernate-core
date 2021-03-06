using System;
using System.Collections;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH266
{
	[TestFixture]
	public class UserFixture : TestCase
	{
		private static int activeId = 1;
		private static int inactiveId = 2;

		protected override string MappingsAssembly
		{
			get { return "NHibernate.Test"; }
		}

		protected override string[] Mappings
		{
			get { return new string[] {"NHSpecificTest.NH266.User.hbm.xml"}; }
		}

		protected override void OnSetUp()
		{
			ISession s = OpenSession();
			User active = new User();
			active.Id = activeId;
			active.Name = "active user";
			active.IsActive = 1;

			User inactive = new User();
			inactive.Id = inactiveId;
			inactive.Name = "inactive user";
			inactive.IsActive = 0;

			s.Save(active);
			s.Save(inactive);

			s.Flush();
			s.Close();
		}

		protected override void OnTearDown()
		{
			ISession s = OpenSession();
			s.Delete("from User");
			s.Flush();
			s.Close();
		}


		/// <summary>
		/// This is testing problems that existed in 0.8.0-2 with extra "AND"
		/// being added to the sql when there was an attribute <c>where="some sql"</c>.
		/// </summary>
		[Test]
		public void WhereAttribute()
		{
			ISession s = OpenSession();
			IList list = s.CreateQuery("from User").List();

			foreach (User u in list)
			{
				Assert.AreEqual(1, u.IsActive, "should only grab active users");
			}

			s.Close();

			// query based on name - ensure that "and" is constructed properly
			s = OpenSession();
			IQuery q = s.CreateQuery("from User as u where u.Name = :name");
			q.SetParameter("name", "active user");
			list = q.List();

			Assert.AreEqual(1, list.Count, "only 1 active user with that name");
			Assert.AreEqual(1, ((User) list[0]).IsActive, "should be active");

			// verify that even a user with a value in the db is 
			// still not found even though a row exists
			q.SetParameter("name", "inactive user");
			list = q.List();
			Assert.AreEqual(0, list.Count, "no 'inactive user' according to where clause");
			s.Close();


//			// load a instance of B through hql
//			s = OpenSession();
//			IQuery q = s.CreateQuery( "from B as b where b.id = :id" );
//			q.SetParameter( "id", bId );
//			b = q.UniqueResult() as B;
//			Assert.AreEqual( "the b", b.Name );
//			s.Close();
//
//			// load a instance of B through Criteria
//			s = OpenSession();
//			ICriteria c = s.CreateCriteria( typeof(B) );
//			c.Add( Expressions.Expression.Eq( "Id", bId ) );
//			b = c.UniqueResult() as B;
//
//			Assert.AreEqual( "the b", b.Name );
//			s.Close();		
		}
	}
}