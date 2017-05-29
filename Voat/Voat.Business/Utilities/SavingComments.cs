#region LICENSE

/*
    
    Copyright(c) Voat, Inc.

    This file is part of Voat.

    This source file is subject to version 3 of the GPL license,
    that is bundled with this package in the file LICENSE, and is
    available online at http://www.gnu.org/licenses/gpl-3.0.txt;
    you may not use this file except in compliance with the License.

    Software distributed under the License is distributed on an
    "AS IS" basis, WITHOUT WARRANTY OF ANY KIND, either express
    or implied. See the License for the specific language governing
    rights and limitations under the License.

    All Rights Reserved.

*/

#endregion LICENSE

using System;
using System.Linq;
using Voat.Data;
using Voat.Data.Models;

namespace Voat.Utilities
{
    [Obsolete("This code is obsolete", true)]
    public class SavingComments
    {
        [Obsolete("Arg Matie, you shipwrecked upon t'is Dead Code", true)]
        // returns true if saved, false otherwise
        public static bool? CheckIfSavedComment(string userToCheck, int commentID)
        {
            using (VoatDataContext db = new VoatDataContext())
            {
                var cmd = db.Connection.CreateCommand();
                cmd.CommandText = "SELECT COUNT(*) FROM CommentSaveTracker WITH (NOLOCK) WHERE UserName = @UserName AND CommentID = @CommentID";

                var param = cmd.CreateParameter();
                param.ParameterName = "UserName";
                param.DbType = System.Data.DbType.String;
                param.Value = userToCheck;
                cmd.Parameters.Add(param);

                param = cmd.CreateParameter();
                param.ParameterName = "CommentID";
                param.DbType = System.Data.DbType.String;
                param.Value = commentID;
                cmd.Parameters.Add(param);

                if (cmd.Connection.State != System.Data.ConnectionState.Open)
                {
                    cmd.Connection.Open();
                }

                int count = (int)cmd.ExecuteScalar();

                return count > 0;
            }

            //using (var db = new voatEntities())
            //{
            //    return db.Commentsavingtrackers.Where(b => b.CommentId == commentId && b.UserName == userToCheck).Any();
            //}
        }
        [Obsolete("Arg Matie, you shipwrecked upon t'is Dead Code", true)]
        // a user wishes to save a comment, save it
        public static void SaveComment(int commentId, string userWhichSaved)
        {
            var result = CheckIfSavedComment(userWhichSaved, commentId);

            using (var db = new VoatDataContext())
            {
                if (result == true)
                {
                    // Already saved, unsave
                    UnSaveComment(userWhichSaved, commentId);
                }
                else
                {
                    // register save
                    var tmpSavingTracker = new CommentSaveTracker
                    {
                        CommentID = commentId,
                        UserName = userWhichSaved,
                        CreationDate = Repository.CurrentDate
                    };
                    db.CommentSaveTracker.Add(tmpSavingTracker);
                    db.SaveChanges();
                }
            }
        }
        [Obsolete("Arg Matie, you shipwrecked upon t'is Dead Code", true)]
        // a user has saved this comment earlier and wishes to unsave it, delete the record
        private static void UnSaveComment(string userWhichSaved, int commentId)
        {
            using (var db = new VoatDataContext())
            {
                var votingTracker = db.CommentSaveTracker.FirstOrDefault(b => b.CommentID == commentId && b.UserName == userWhichSaved);

                if (votingTracker == null)
                    return;

                // delete vote history
                db.CommentSaveTracker.Remove(votingTracker);
                db.SaveChanges();
            }
        }
    }
}
