### New Rules

 Rule ID   | Category      | Severity | Notes                                                                     
-----------|---------------|----------|---------------------------------------------------------------------------
 AOM100    | Usage         | Error    | Method has incorrect return type                                          
 AOM101    | Usage         | Error    | Method has incorrect source parameter type                                
 AOM102    | Usage         | Error    | Method has incorrect MapperContext parameter type                         
 AOM103    | Performance   | Warning  | Method does not require MapperContext and the parameter should be removed 
 AOM104    | Usage         | Error    | Method must be static                                                     
 AOM200    | Configuration | Error    | Maps must be distinct                                                     
 AOM201    | Configuration | Error    | Member names should be valid                                              
 AOM202    | Design        | Warning  | Prefer using nameof() over raw string                                     
 AOM203    | Usage         | Error    | UseFormatProvider destination type should be valid type                   
 AOM204    | Design        | Warning  | Potential recursive mapping detected                                      
 AOM205    | Usage         | Error    | Mapper should only have one default FormatProvider                        
 AOM206    | Usage         | Error    | Mapper should only have one FormatProvider for type pair                  
 AOM208    | Configuration | Warning  | Ignored member does not exist                                             
 AOM209    | Configuration | Error    | Duplicate configuration for member                                        
 AOM300    | TypeSafety    | Warning  | No map found for destination type                                         
 AOM301    | TypeSafety    | Warning  | Nullable assignment to non-nullable property                              
 AOM302    | TypeSafety    | Warning  | Nullable assignment to non-nullable property                              
 AOM400    | Configuration | Warning  | Unmapped destination member                                               
 AOM401    | Design        | Error    | Property not publicly settable                                            
 AOM500    | TypeSafety    | Warning  | Potential loss of data when converting between types                      
 AOM501    | TypeSafety    | Warning  | Enum value does not exist on destination                                  
 AOM502    | TypeSafety    | Warning  | Enum value does not exist on destination                                  
